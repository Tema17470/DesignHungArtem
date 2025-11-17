using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface INotificationAdapter
    {
        Task NotifyAsync(int deviceId, string title, string message, CancellationToken ct = default);
    }
}
