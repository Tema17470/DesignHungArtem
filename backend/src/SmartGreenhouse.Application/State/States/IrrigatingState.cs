using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Application.State.States
{
    public class IrrigatingState : IGreenhouseState
    {
        public Task<GreenhouseStateEngine.TransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
        {
            var soilmoistureReading = context.LatestReadings.FirstOrDefault(r => r.SensorType == SensorTypeEnum.SoilMoisture);
            if (soilmoistureReading != null && soilmoistureReading.Value >= context.SoilMoistureThreshold)
            {
                return Task.FromResult(new GreenhouseStateEngine.TransitionResult("Idle", new List<ActuatorCommand>(), "Soil moisture sufficient → return to idle"));
            }

            return Task.FromResult(new GreenhouseStateEngine.TransitionResult("Irrigating",
                new List<ActuatorCommand> { new ActuatorCommand("Pump", "On") },
                "Irrigating → Pump On"));
        }
    }
}
