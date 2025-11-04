namespace SmartGreenhouse.Api.Contracts;

public record UpsertAlertRuleRequest(
    int DeviceId,
    SensorTypeEnum SensorType,
    string OperatorSymbol,
    double Threshold,
    bool IsActive
);
