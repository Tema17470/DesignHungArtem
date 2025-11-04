namespace SmartGreenhouse.Domain.Entities;

public class ControlProfile
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public Device? Device { get; set; }
    public string StrategyKey { get; set; } = string.Empty;
    public string? ParametersJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
