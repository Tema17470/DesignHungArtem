using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Application.State.States
{
    public class CoolingState : IGreenhouseState
    {
        public Task<GreenhouseStateEngine.TransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
        {
            var tempReading = context.LatestReadings.FirstOrDefault(r => r.SensorType == SensorTypeEnum.Temperature);
            if (tempReading != null && tempReading.Value <= context.TemperatureThreshold)
            {
                return Task.FromResult(new GreenhouseStateEngine.TransitionResult("Idle", new List<ActuatorCommand>(), "Temperature normal → return to idle"));
            }

            return Task.FromResult(new GreenhouseStateEngine.TransitionResult("Cooling",
                new List<ActuatorCommand> { new ActuatorCommand("Fan", "On") },
                "Cooling → Fan On"));
        }
    }
}
