using System.Collections.Generic;
using SmartGreenhouse.Domain.Entities;
using Application.Control;

namespace SmartGreenhouse.Application.State
{
    public class GreenhouseStateContext
    {
        public int DeviceId { get; init; }
        public IReadOnlyList<SensorReading> LatestReadings { get; init; } = new List<SensorReading>();
        
        // Thresholds
        public double TemperatureThreshold { get; init; } = 26.0;
        public double SoilMoistureThreshold { get; init; } = 30.0;

        public GreenhouseStateContext(int deviceId, IReadOnlyList<SensorReading>? readings = null)
        {
            DeviceId = deviceId;
            if (readings != null)
                LatestReadings = readings;
        }
    }
}
