using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;

namespace Application.Events.Observers
{
    public class LogObserver : IReadingObserver
    {
        public Task OnReadingAsync(IReadingEvent readingEvent, CancellationToken ct = default)
        {
            Console.WriteLine(
                $"[LogObserver] Device:{readingEvent.DeviceId}, " +
                $"Sensor:{readingEvent.SensorType}, " +
                $"Value:{readingEvent.Value}, " +
                $"Time:{readingEvent.Timestamp:O}"
            );
            return Task.CompletedTask;
        }
    }
}
