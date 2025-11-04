namespace SmartGreenhouse.Api.Contracts;

public record SetControlProfileRequest(
    int DeviceId,
    string StrategyKey,
    Dictionary<string, double>? Parameters
);
