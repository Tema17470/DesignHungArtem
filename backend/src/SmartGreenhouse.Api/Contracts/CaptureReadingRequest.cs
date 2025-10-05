using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Api.Contracts
{
    public record CaptureReadingRequest(
        int DeviceId,
        SensorTypeEnum SensorType
    );
}