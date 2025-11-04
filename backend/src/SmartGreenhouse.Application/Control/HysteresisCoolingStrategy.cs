using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;

namespace Application.Control
{
    public sealed class HysteresisCoolingStrategy : IControlStrategy
    {
        private const double UpperThreshold = 26.0;
        private const double LowerThreshold = 22.0;

        public async Task EvaluateAsync(int deviceId, double value, IActuatorController controller, CancellationToken ct = default)
        {
            if (value > UpperThreshold)
            {
                await controller.SetStateAsync(deviceId, "CoolingFan", on: true, ct);
                Console.WriteLine($"[CTRL] Cooling ON (Temp {value:F1}°C > {UpperThreshold})");
            }
            else if (value < LowerThreshold)
            {
                await controller.SetStateAsync(deviceId, "CoolingFan", on: false, ct);
                Console.WriteLine($"[CTRL] Cooling OFF (Temp {value:F1}°C < {LowerThreshold})");
            }
        }
    }
}
