using SmartGreenhouse.Domain.Entities;

namespace Api.Contracts
{
    public record CaptureReadingRequest(
        int DeviceId,
        SensorTypeEnum SensorType
    );
}