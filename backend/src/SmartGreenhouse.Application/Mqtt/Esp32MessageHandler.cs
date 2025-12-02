using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartGreenhouse.Infrastructure.Data;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Domain.Enums;
using SmartGreenhouse.Application.Contracts;
using SmartGreenhouse.Application.RealTime;

namespace SmartGreenhouse.Application.Mqtt
{
    public class Esp32MessageHandler : IEsp32MessageHandler
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly ILogger<Esp32MessageHandler> _logger;
        private readonly IRealTimeNotifier? _realTimeNotifier;

        public Esp32MessageHandler(IDbContextFactory<AppDbContext> dbFactory,
                                   ILogger<Esp32MessageHandler> logger,
                                   IRealTimeNotifier? realTimeNotifier)
        {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _realTimeNotifier = realTimeNotifier; // may be null in some registrations
        }

        public async Task HandleAsync(string topic, string payload, CancellationToken ct = default)
        {
            _logger.LogInformation("MQTT message received on topic '{Topic}': {Payload}", topic, payload);

            if (string.IsNullOrWhiteSpace(payload))
            {
                _logger.LogWarning("Empty payload received on topic {Topic}", topic);
                return;
            }

            try
            {
                // Expected topic format: greenhouse/{deviceName}/sensor/{sensorType}
                var parts = (topic ?? string.Empty).Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4 && string.Equals(parts[0], "greenhouse", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(parts[2], "sensor", StringComparison.OrdinalIgnoreCase))
                {
                    var deviceName = parts[1];
                    var sensorTypeText = parts[3];

                    if (!Enum.TryParse<SensorTypeEnum>(sensorTypeText, true, out var sensorType))
                    {
                        _logger.LogWarning("Unknown sensor type '{SensorTypeText}' in topic {Topic}", sensorTypeText, topic);
                        return;
                    }

                    // Deserialize payload into Esp32Payload (case-insensitive)
                    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var espPayload = JsonSerializer.Deserialize<Esp32Payload>(payload, opts);
                    if (espPayload == null)
                    {
                        _logger.LogWarning("Payload deserialized to null for topic {Topic}", topic);
                        return;
                    }

                    using var db = _dbFactory.CreateDbContext();

                    // Find device by name; if missing create it
                    var device = await db.Devices.FirstOrDefaultAsync(d => d.DeviceName == deviceName, ct);
                    if (device == null)
                    {
                        device = new Device
                        {
                            DeviceName = deviceName,
                            DeviceType = DeviceTypeEnum.MqttEdge,
                            CreatedAt = DateTime.UtcNow
                        };
                        db.Devices.Add(device);
                        await db.SaveChangesAsync(ct); // persist so we have an Id
                        _logger.LogInformation("Created new device '{DeviceName}' with Id {DeviceId}", deviceName, device.Id);
                    }

                    var reading = new SensorReading
                    {
                        DeviceId = device.Id,
                        SensorType = sensorType,
                        Value = espPayload.Value,
                        Unit = espPayload.Unit ?? string.Empty,
                        Timestamp = espPayload.Timestamp ?? DateTime.UtcNow
                    };

                    db.Readings.Add(reading);
                    await db.SaveChangesAsync(ct);

                    _logger.LogInformation("Stored sensor reading (Device={DeviceName}, Id={DeviceId}, Type={Type}, Value={Value})",
                        deviceName, device.Id, sensorType, espPayload.Value);

                    // Map to DTO and broadcast to real-time clients if available
                    var dtoOut = MapToDto(reading, device);
                    if (_realTimeNotifier != null)
                    {
                        try
                        {
                            await _realTimeNotifier.BroadcastReadingAsync(dtoOut, ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Broadcasting reading failed for ReadingId={ReadingId}", reading.Id);
                        }
                    }
                }
                else
                {
                    _logger.LogDebug("Topic does not match expected pattern: {Topic}", topic);
                }
            }
            catch (JsonException je)
            {
                _logger.LogWarning(je, "Failed to parse MQTT payload as JSON: {Payload}", payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MQTT message on topic {Topic}", topic);
            }
        }

        // Internal payload model for ESP32 JSON messages
    private sealed class Esp32Payload
{
    public double Value { get; set; }
    public string Unit { get; set; } = "";
    public DateTime? Timestamp { get; set; }
}

        private ReadingDto MapToDto(SensorReading reading, Device device)
            => new(
                reading.Id,
                reading.DeviceId,
                device.DeviceName,
                reading.SensorType,
                reading.Value,
                reading.Unit,
                reading.Timestamp
            );
        
    }
    
}
