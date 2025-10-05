using System;
using Application.Abstractions;
using Application.Factories;
using SmartGreenhouse.Domain.Enums;

namespace Application.DeviceIntegration;

public interface IDeviceFactoryResolver
{
    IDeviceIntegrationFactory Resolve(DeviceTypeEnum deviceType);
}

public class DeviceFactoryResolver : IDeviceFactoryResolver
{
    private readonly IServiceProvider _serviceProvider;

    public DeviceFactoryResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IDeviceIntegrationFactory Resolve(DeviceTypeEnum deviceType)
    {
        return deviceType switch
        {
            DeviceTypeEnum.Simulated => (IDeviceIntegrationFactory)_serviceProvider.GetService(typeof(SimulatedDeviceFactory))!,
            _ => throw new NotSupportedException($"Device type '{deviceType}' is not supported.")
        };
    }
}