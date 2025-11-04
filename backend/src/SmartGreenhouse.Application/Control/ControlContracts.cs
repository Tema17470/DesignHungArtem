using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;

namespace Application.Control
{
    public interface IControlStrategy
    {
        Task EvaluateAsync(int deviceId, double value, IActuatorController controller, CancellationToken ct = default);
    }

    public interface IControlStrategySelector
    {
        IControlStrategy Select(SensorTypeEnum sensorType);
    }
}
