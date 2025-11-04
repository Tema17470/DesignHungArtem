using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.DeviceIntegration;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Enums;
using SmartGreenhouse.Infrastructure.Data;

namespace Application.Control
{
    public sealed class ControlService
    {
        private readonly IControlStrategySelector _selector;
        private readonly IDeviceFactoryResolver _resolver;
        private readonly AppDbContext _db;

        public ControlService(IControlStrategySelector selector, IDeviceFactoryResolver resolver, AppDbContext db)
        {
            _selector = selector;
            _resolver = resolver;
            _db = db;
        }

        public async Task EvaluateAsync(int deviceId, CancellationToken ct = default)
        {
            var latest = await _db.Readings
                .Where(r => r.DeviceId == deviceId)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync(ct);

            if (latest == null)
                throw new InvalidOperationException($"No readings found for device {deviceId}");

            var strategy = _selector.Select(latest.SensorType);
            var factory = _resolver.Resolve(DeviceTypeEnum.Simulated);
            var controller = factory.CreateActuatorController();

            await strategy.EvaluateAsync(deviceId, latest.Value, controller, ct);
        }
    }
}
