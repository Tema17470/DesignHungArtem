using Application.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.Adapters.Notifications
{
    public class ConsoleNotificationAdapter : INotificationAdapter
    {
        public Task NotifyAsync(int deviceId, string title, string message, CancellationToken ct = default)
        {
            Console.WriteLine($"[Notification] Device {deviceId}: {title} - {message}");
            return Task.CompletedTask;
        }
    }
}
