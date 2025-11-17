using System.Threading;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.State
{
    public interface IGreenhouseState
    {
        Task<GreenhouseStateEngine.TransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default);
    }
}
