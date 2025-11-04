using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Domain.Entities;

public class AlertRule
{
    public int Id { get; set; } 
    public int DeviceId { get; set; }
    public SensorTypeEnum SensorType { get; set; }
    public string OperatorSymbol { get; set; } = string.Empty;
    public double Threshold { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Device? Device { get; set; }
}
