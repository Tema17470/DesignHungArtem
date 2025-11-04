using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Infrastructure.Data;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AlertsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/alerts?deviceId=1
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? deviceId = null, [FromQuery] int take = 200)
    {
        var q = _db.AlertNotifications
            .AsNoTracking()
            .OrderByDescending(a => a.Timestamp)
            .AsQueryable();

        if (deviceId.HasValue) q = q.Where(a => a.DeviceId == deviceId.Value);

        var list = await q.Take(take).ToListAsync();
        return Ok(list);
    }
}
