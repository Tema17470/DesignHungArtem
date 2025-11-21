using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Api.Contracts
{
    public class CaptureReadingRequest
    {
        public int DeviceId { get; set; }
        public SensorTypeEnum SensorType { get; set; }
        public double? Value { get; set; } 
        public string? Unit { get; set; }  
    }
}