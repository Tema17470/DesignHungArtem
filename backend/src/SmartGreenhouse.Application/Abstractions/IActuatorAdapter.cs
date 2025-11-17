using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Application.State;

namespace Application.Abstractions
{
    public interface IActuatorAdapter
    {
        Task ApplyAsync(int deviceId, IReadOnlyList<ActuatorCommand> commands, CancellationToken ct = default);
    }
}
