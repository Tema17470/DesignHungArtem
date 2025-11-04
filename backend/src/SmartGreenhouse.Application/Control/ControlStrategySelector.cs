using System;
using Application.Abstractions;

namespace Application.Control
{
    public sealed class ControlStrategySelector : IControlStrategySelector
    {
        private readonly HysteresisCoolingStrategy _cooling;
        private readonly MoistureTopUpStrategy _moisture;

        public ControlStrategySelector(HysteresisCoolingStrategy cooling, MoistureTopUpStrategy moisture)
        {
            _cooling = cooling;
            _moisture = moisture;
        }

        public IControlStrategy Select(SensorTypeEnum sensorType)
        {
            return sensorType switch
            {
                SensorTypeEnum.Temperature => _cooling,
                SensorTypeEnum.SoilMoisture => _moisture,
                _ => throw new NotSupportedException($"No control strategy defined for {sensorType}")
            };
        }
    }
}
