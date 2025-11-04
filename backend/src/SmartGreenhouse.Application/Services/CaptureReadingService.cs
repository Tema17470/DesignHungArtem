using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Factories;
using Application.Events;
using Application.DeviceIntegration;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Infrastructure.Data;
using SmartGreenhouse.Domain.Entities;


namespace SmartGreenhouse.Application.Services
{
    public class CaptureReadingService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDeviceFactoryResolver _deviceFactoryResolver;
        private readonly IReadingPublisher _publisher;

        public CaptureReadingService(AppDbContext dbContext, IDeviceFactoryResolver deviceFactoryResolver, IReadingPublisher publisher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _deviceFactoryResolver = deviceFactoryResolver ?? throw new ArgumentNullException(nameof(deviceFactoryResolver));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task<SensorReading> CaptureAsync(int deviceId, SensorTypeEnum sensorType, CancellationToken ct = default)
        {
            // Load device from DB
            var device = await _dbContext.Devices
                .FirstOrDefaultAsync(d => d.Id == deviceId, ct);

            if (device == null)
                throw new InvalidOperationException($"Device with ID {deviceId} not found.");

            // Resolve the device factory
            var factory = _deviceFactoryResolver.Resolve(device.DeviceType);

            // Get sensor reader
            var sensorReader = factory.CreateSensorReader();

            // Read raw value
            double rawValue = await sensorReader.ReadAsync(deviceId, sensorType, ct);

            // Normalize
            var normalizer = SensorNormalizerFactory.Create(sensorType);
            double normalizedValue = normalizer.Normalize(rawValue);

            // Create sensor reading
            var reading = new SensorReading
            {
                DeviceId = deviceId,
                SensorType = sensorType,          // enum
                Value = normalizedValue,
                Unit = normalizer.CanonicalUnit,
                Timestamp = DateTime.UtcNow
            };

            // Save to database
            _dbContext.Readings.Add(reading);
            await _dbContext.SaveChangesAsync(ct);

            // Publishing reading
            var readingEvent = new ReadingEvent
            (
                reading.DeviceId,
                reading.SensorType,
                reading.Value,
                reading.Timestamp
            );

            await _publisher.PublishAsync(readingEvent, ct);

            return reading;
        }
    }
}
