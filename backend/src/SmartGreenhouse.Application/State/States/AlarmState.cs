using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.State.States
{
    public class AlarmState : IGreenhouseState
    {
        public async Task<GreenhouseStateEngine.TransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
        {
            var commands = new List<ActuatorCommand>
            {
                new ActuatorCommand("Fan", "Off"),
                new ActuatorCommand("Pump", "Off")
            };

            // Notify (using NotificationAdapter in engine via context or injected)
            // For now, we just add a note
            string note = "Alarm → safe actions, notify operators";

            return new GreenhouseStateEngine.TransitionResult("Idle", commands, note);
        }
    }
}
