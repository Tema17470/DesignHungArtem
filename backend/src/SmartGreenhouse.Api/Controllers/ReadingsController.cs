using Microsoft.AspNetCore.Mvc;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;
using SmartGreenhouse.Api.Contracts;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CaptureReadingService _capture;    
    private readonly ReadingService _reading;
    public ReadingsController(AppDbContext db, CaptureReadingService capture, ReadingService reading)
    {
        _db = db;
        _capture = capture;
        _reading = reading;
    }

    // GET /api/readings?deviceId=1&sensorType=Temperature
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? deviceId, [FromQuery] SensorTypeEnum? sensorType)
    {
        var readings = await _reading.QueryAsync(deviceId, sensorType);
        var dtos = readings.Select(r => new ReadingDto(r.Id, r.DeviceId, r.SensorType, r.Value, r.Unit, r.Timestamp));
        return Ok(dtos);
    }

    // POST /api/readings/capture
    [HttpPost("capture")]
    public async Task<IActionResult> Capture([FromBody] CaptureReadingRequest req)
    {
        var r = await _capture.CaptureAsync(req.DeviceId, req.SensorType, req.Value, req.Unit);
        return Ok(new ReadingDto(r.Id, r.DeviceId, r.SensorType, r.Value, r.Unit, r.Timestamp));
    }
}