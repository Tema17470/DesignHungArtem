using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;

namespace Application.Control
{
    public sealed class MoistureTopUpStrategy : IControlStrategy
    {
        private const double MinThreshold = 30.0;
        private const double MaxThreshold = 50.0;

        public async Task EvaluateAsync(int deviceId, double value, IActuatorController controller, CancellationToken ct = default)
        {
            if (value < MinThreshold)
            {
                await controller.SetStateAsync(deviceId, "WaterPump", on: true, ct);
                Console.WriteLine($"[CTRL] WaterPump ON (Soil {value:F1}% < {MinThreshold}%)");
            }
            else if (value > MaxThreshold)
            {
                await controller.SetStateAsync(deviceId, "WaterPump", on: false, ct);
                Console.WriteLine($"[CTRL] WaterPump OFF (Soil {value:F1}% > {MaxThreshold}%)");
            }
        }
    }
}
