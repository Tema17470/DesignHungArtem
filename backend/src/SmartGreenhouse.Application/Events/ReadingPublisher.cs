using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;

namespace Application.Events
{
    public class ReadingEvent : IReadingEvent
    {
        public int DeviceId { get; }
        public SensorTypeEnum SensorType { get; }
        public double Value { get; }
        public DateTime Timestamp { get; }

        public ReadingEvent(int deviceId, SensorTypeEnum sensorType, double value, DateTime timestamp)
        {
            DeviceId = deviceId;
            SensorType = sensorType;
            Value = value;
            Timestamp = timestamp;
        }
    }

    public class ReadingPublisher : IReadingPublisher
    {
        private readonly IEnumerable<IReadingObserver> _observers;

        public ReadingPublisher(IEnumerable<IReadingObserver> observers)
        {
            _observers = observers;
        }

        public async Task PublishAsync(IReadingEvent readingEvent, CancellationToken ct = default)
        {
            foreach (var observer in _observers)
            {
                await observer.OnReadingAsync(readingEvent, ct);
            }
        }
    }
}
