using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Application.State;

namespace SmartGreenhouse.Application.Adapters.Actuators
{
    public class SimulatedActuatorAdapter : IActuatorAdapter
    {
        public Task ApplyAsync(int deviceId, IReadOnlyList<ActuatorCommand> commands, CancellationToken ct = default)
        {
            foreach (var cmd in commands)
            {
                Console.WriteLine($"[SimulatedActuator] Device {deviceId}: {cmd.ActuatorName} -> {cmd.Action}");
            }
            return Task.CompletedTask;
        }
    }
}
