using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Domain.Entities;

public class AlertNotification
{
    public int Id { get; set; } 
    public int? AlertRuleId { get; set; }
    public AlertRule? AlertRule { get; set; }
    public int DeviceId { get; set; }
    public SensorTypeEnum SensorType { get; set; }
    public double Value { get; set; }
    public double Threshold { get; set; }
    public string OperatorSymbol { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
