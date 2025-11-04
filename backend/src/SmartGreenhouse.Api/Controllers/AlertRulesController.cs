using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertRulesController : ControllerBase
{
    private readonly AppDbContext _db;

    public AlertRulesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertAlertRuleRequest req)
    {
        var rule = await _db.AlertRules
            .FirstOrDefaultAsync(r => r.DeviceId == req.DeviceId && r.SensorType == req.SensorType);

        if (rule == null)
        {
            rule = new AlertRule
            {
                DeviceId = req.DeviceId,
                SensorType = req.SensorType,
                OperatorSymbol = req.OperatorSymbol,
                Threshold = req.Threshold,
                IsActive = req.IsActive
            };
            _db.AlertRules.Add(rule);
        }
        else
        {
            rule.OperatorSymbol = req.OperatorSymbol;
            rule.Threshold = req.Threshold;
            rule.IsActive = req.IsActive;
        }

        await _db.SaveChangesAsync();
        return Ok(rule);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? deviceId = null)
    {
        var q = _db.AlertRules.AsQueryable();
        if (deviceId.HasValue)
            q = q.Where(r => r.DeviceId == deviceId.Value);

        var result = await q.AsNoTracking().ToListAsync();
        return Ok(result);
    }
}
