using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.State.States
{
    public class AlarmState : IGreenhouseState
    {
        public async Task<GreenhouseStateEngine.TransitionResult> TickAsync(
            GreenhouseStateContext context, 
            CancellationToken ct = default)
        {
            // Always put devices in safe state
            var commands = new List<ActuatorCommand>
            {
                new ActuatorCommand("Fan", "Off"),
                new ActuatorCommand("Pump", "Off")
            };

            // Notify via the adapter from context
            string title = $"Device {context.DeviceId} entered ALARM state!";
            string message = $"Sensor readings are out of safe range. Fan and Pump turned off.";
            
            if (context.NotificationAdapter != null)
            {
                await context.NotificationAdapter.NotifyAsync(context.DeviceId, title, message, ct);
            }

            // Return the next state + commands + optional note
            string note = "Alarm → safe actions applied and notification sent";
            return new GreenhouseStateEngine.TransitionResult("Idle", commands, note);
        }
    }
}
