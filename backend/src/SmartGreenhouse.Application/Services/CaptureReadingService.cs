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

        public async Task<SensorReading> CaptureAsync(
    int deviceId, 
    SensorTypeEnum sensorType, 
    double? value = null,    
    string? unit = null,       
    CancellationToken ct = default)
{
    // Load device from DB
    var device = await _dbContext.Devices
        .FirstOrDefaultAsync(d => d.Id == deviceId, ct);

    if (device == null)
        throw new InvalidOperationException($"Device with ID {deviceId} not found.");

    double normalizedValue;
    string canonicalUnit;

    if (value.HasValue)
    {
        normalizedValue = value.Value;
        canonicalUnit = unit; 
    }
    else
    {
        var factory = _deviceFactoryResolver.Resolve(device.DeviceType);
        var sensorReader = factory.CreateSensorReader();
        double rawValue = await sensorReader.ReadAsync(deviceId, sensorType, ct);
        var normalizer = SensorNormalizerFactory.Create(sensorType);
        normalizedValue = normalizer.Normalize(rawValue);
        canonicalUnit = normalizer.CanonicalUnit;
    }

    // Create sensor reading
    var reading = new SensorReading
    {
        DeviceId = deviceId,
        SensorType = sensorType,
        Value = normalizedValue,
        Unit = canonicalUnit,
        Timestamp = DateTime.UtcNow
    };

    _dbContext.Readings.Add(reading);
    await _dbContext.SaveChangesAsync(ct);

    // Publish reading
    var readingEvent = new ReadingEvent(
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
