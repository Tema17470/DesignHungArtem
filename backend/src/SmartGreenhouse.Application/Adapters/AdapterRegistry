using System;
using Application.Abstractions;

namespace SmartGreenhouse.Application.Adapters
{
    public class AdapterRegistry
    {
        public IActuatorAdapter ActuatorAdapter { get; private set; }
        public INotificationAdapter NotificationAdapter { get; private set; }

        public AdapterRegistry(IActuatorAdapter defaultActuator, INotificationAdapter defaultNotification)
        {
            ActuatorAdapter = defaultActuator;
            NotificationAdapter = defaultNotification;
        }

        public void SetActuatorAdapter(IActuatorAdapter adapter) => ActuatorAdapter = adapter;
        public void SetNotificationAdapter(INotificationAdapter adapter) => NotificationAdapter = adapter;
    }
}
