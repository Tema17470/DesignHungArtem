namespace Application.Abstractions;

public interface IDeviceIntegrationFactory
{
    ISensorReader CreateSensorReader();
    IActuatorController CreateActuatorController();
}
