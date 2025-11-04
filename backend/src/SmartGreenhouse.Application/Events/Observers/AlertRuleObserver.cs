using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace Application.Events.Observers
{
    public class AlertRuleObserver : IReadingObserver
    {
        private readonly AppDbContext _db;

        public AlertRuleObserver(AppDbContext db)
        {
            _db = db;
        }

        public async Task OnReadingAsync(IReadingEvent readingEvent, CancellationToken ct = default)
        {
            // Get active rules matching the device + sensor
            var rules = await _db.Set<AlertRule>()
                .Where(r => r.DeviceId == readingEvent.DeviceId
                            && r.SensorType == readingEvent.SensorType
                            && r.IsActive)
                .ToListAsync(ct);

            foreach (var rule in rules)
            {
                bool triggered = rule.OperatorSymbol switch
                {
                    ">"  => readingEvent.Value > rule.Threshold,
                    "<"  => readingEvent.Value < rule.Threshold,
                    ">=" => readingEvent.Value >= rule.Threshold,
                    "<=" => readingEvent.Value <= rule.Threshold,
                    "==" => Math.Abs(readingEvent.Value - rule.Threshold) < 0.0001,
                    _    => false
                };

                if (triggered)
                {
                    var alert = new AlertNotification
                    {
                        DeviceId = readingEvent.DeviceId,
                        SensorType = readingEvent.SensorType,
                        Message = $"Threshold breached ({rule.OperatorSymbol}{rule.Threshold})",
                        Timestamp = readingEvent.Timestamp
                    };
                    _db.Set<AlertNotification>().Add(alert);
                }
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
