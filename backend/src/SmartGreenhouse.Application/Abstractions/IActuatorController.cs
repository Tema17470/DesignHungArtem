using System.Threading;
using System.Threading.Tasks;
namespace Application.Abstractions;

public interface IActuatorController
{
    Task SetStateAsync(int deviceId, string actuatorName, bool on, CancellationToken ct = default);
}
