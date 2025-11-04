using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Domain.Enums;

namespace Application.Abstractions
{
    public interface IReadingEvent
    {
        int DeviceId { get; }
        SensorTypeEnum SensorType { get; }
        double Value { get; }
        DateTime Timestamp { get; }
    }

    public interface IReadingObserver
    {
        Task OnReadingAsync(IReadingEvent readingEvent, CancellationToken ct = default);
    }

    public interface IReadingPublisher
    {
        Task PublishAsync(IReadingEvent readingEvent, CancellationToken ct = default);
    }
}
