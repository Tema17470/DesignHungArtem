using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    /// <summary>
    /// Minimal real-time notifier contract used by application components to notify
    /// connected clients (e.g. via SignalR or websockets). This is intentionally
    /// small and can be expanded in your Part 2 implementation.
    /// </summary>
    public interface IRealTimeNotifier
    {
        Task NotifyDeviceAsync(int deviceId, string eventName, object? payload = null, CancellationToken ct = default);
    }
}
