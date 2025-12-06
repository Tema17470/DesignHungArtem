using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;
using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.State;

namespace SmartGreenhouse.Application.Services
{
    public class StateService
    {
        private readonly AppDbContext _db;
        private readonly GreenhouseStateEngine _engine;
        private readonly IActuatorAdapter _actuatorAdapter;
        private readonly INotificationAdapter _notificationAdapter; 

        
        public StateService(AppDbContext db, GreenhouseStateEngine engine, IActuatorAdapter actuatorAdapter, INotificationAdapter notificationAdapter)
        {
            _db = db;
            _engine = engine;
            _actuatorAdapter = actuatorAdapter;
            _notificationAdapter = notificationAdapter;
        }

        public async Task<GreenhouseStateEngine.TransitionResult> TickAsync(int deviceId, CancellationToken ct = default)
        {
            // Get latest readings
            var latestReadings = await _db.Readings
            .Where(r => r.DeviceId == deviceId)
            .GroupBy(r => r.SensorType)
            .Select(g => g.OrderByDescending(r => r.Timestamp).First())
            .ToListAsync(ct);


            // Get last saved state
            var lastSnapshot = await _db.DeviceStates
                .Where(s => s.DeviceId == deviceId)
                .OrderByDescending(s => s.EnteredAt)
                .FirstOrDefaultAsync(ct);

            string currentStateName = lastSnapshot?.StateName ?? "Idle";

            // Create context
            var context = new State.GreenhouseStateContext(
                deviceId,
                latestReadings,
                _actuatorAdapter,
                _notificationAdapter
            );

            // Resolve the current state
            IGreenhouseState state = currentStateName switch
            {
                "Idle" => new State.States.IdleState(),
                "Cooling" => new State.States.CoolingState(),
                "Irrigating" => new State.States.IrrigatingState(),
                "Alarm" => new State.States.AlarmState(),
                _ => new State.States.IdleState()
            };


            // Tick
            var result = await _engine.TickAsync(deviceId, state, context, ct);

            // Apply actuator commands
            if (result.Commands?.Any() == true)
            {
                await _actuatorAdapter.ApplyAsync(deviceId, result.Commands, ct);
            }

            // Persist new snapshot if state changed
            if (result.NextStateName != currentStateName)
            {
                var snapshot = new DeviceStateSnapshot
                {
                    DeviceId = deviceId,
                    StateName = result.NextStateName,
                    EnteredAt = DateTime.UtcNow,
                    Notes = result.Note
                };

                _db.DeviceStates.Add(snapshot);
                await _db.SaveChangesAsync(ct);
            }

            return result;
        }
    }
}
