using System.Collections.Generic;
using SmartGreenhouse.Domain.Entities;
using Application.Control;
using Application.Abstractions;

namespace SmartGreenhouse.Application.State
{
    public class GreenhouseStateContext
    {
        public int DeviceId { get; init; }
        public IReadOnlyList<SensorReading> LatestReadings { get; init; } = new List<SensorReading>();
        
        // Thresholds
        public double TemperatureThreshold { get; init; } = 26.0;
        public double SoilMoistureThreshold { get; init; } = 30.0;

        // Add adapters
        public IActuatorAdapter ActuatorAdapter { get; init; } = null!;
        public INotificationAdapter NotificationAdapter { get; init; } = null!;

        public GreenhouseStateContext(
            int deviceId,
            IReadOnlyList<SensorReading>? readings = null,
            IActuatorAdapter? actuatorAdapter = null,
            INotificationAdapter? notificationAdapter = null)
        {
            DeviceId = deviceId;
            if (readings != null)
                LatestReadings = readings;

            if (actuatorAdapter != null)
                ActuatorAdapter = actuatorAdapter;
            if (notificationAdapter != null)
                NotificationAdapter = notificationAdapter;
        }
    }
}
