using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Application.State.States
{
    public class IdleState : IGreenhouseState
    {
        public Task<GreenhouseStateEngine.TransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
        {
            bool tempHigh = context.LatestReadings.Any(r => r.SensorType == SensorTypeEnum.Temperature && r.Value > context.TemperatureThreshold);
            bool soilmoistureLow = context.LatestReadings.Any(r => r.SensorType == SensorTypeEnum.SoilMoisture && r.Value < context.SoilMoistureThreshold);

            if (tempHigh)
                return Task.FromResult(new GreenhouseStateEngine.TransitionResult("Cooling",
                new List<ActuatorCommand>{ new ActuatorCommand("Fan", "On") }, 
                "Temperature high → start cooling"));

            if (soilmoistureLow)
                return Task.FromResult(new GreenhouseStateEngine.TransitionResult("Irrigating", 
                new List<ActuatorCommand>{ new ActuatorCommand("Pump", "On") }, 
                "Soil moisture low → start irrigation"));

            return Task.FromResult(new GreenhouseStateEngine.TransitionResult("Idle", 
            new List<ActuatorCommand>{new ActuatorCommand("Fan", "Off"), new ActuatorCommand("Pump", "Off")}, 
            "Idle → no action"));
        }
    }
}
