using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartGreenhouse.Domain.Entities;
using Application.Abstractions;

namespace SmartGreenhouse.Application.State
{
    
    // ActuatorCommand will be defined here for lack of better place
    public record ActuatorCommand(string ActuatorName, string Action);
    
    public class GreenhouseStateEngine
    {
        private readonly IActuatorAdapter _actuatorAdapter;
        private readonly INotificationAdapter _notificationAdapter;

        public GreenhouseStateEngine(IActuatorAdapter actuatorAdapter, INotificationAdapter notificationAdapter)
        {
            _actuatorAdapter = actuatorAdapter;
            _notificationAdapter = notificationAdapter;
        }

        // TransitionResult will be defined here for lack of better place
        public record TransitionResult(
            string NextStateName,
            List<ActuatorCommand> Commands,
            string? Note = null
        );

        public async Task<TransitionResult> TickAsync(int deviceId, IGreenhouseState state, GreenhouseStateContext context, CancellationToken ct = default)
        {
            return await state.TickAsync(context, ct);
        }
    }
}
