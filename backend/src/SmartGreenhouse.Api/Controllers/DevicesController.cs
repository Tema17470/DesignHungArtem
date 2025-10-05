using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;
using SmartGreenhouse.Api.Contracts;
namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    public DevicesController(AppDbContext db) => _db = db;

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> Get() 
    {
        return Ok(await _db.Devices.AsNoTracking().Select(d => new DeviceDto(d.Id, d.DeviceName, d.DeviceType, d.CreatedAt)).ToListAsync());
    }


    [HttpPost]
    public async Task<IActionResult> Create(Device device)
    {
        _db.Devices.Add(device);
        await _db.SaveChangesAsync();
        return Ok(device);
    }
}