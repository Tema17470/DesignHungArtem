using Microsoft.AspNetCore.Mvc;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Application.State;
using SmartGreenhouse.Application.Adapters;
using SmartGreenhouse.Application.Adapters.Actuators;
using SmartGreenhouse.Application.Adapters.Notifications;
using SmartGreenhouse.Application.Services;

namespace SmartGreenhouse.Api.Controllers
{
    [ApiController]
[Route("api/state")]
public class StateController : ControllerBase
{
    private readonly GreenhouseStateEngine _engine;
    private readonly StateService _stateService;
    private readonly AdapterRegistry _registry;
    private readonly IHttpClientFactory _httpClientFactory;

    public StateController(
        GreenhouseStateEngine engine,
        StateService stateService,
        AdapterRegistry registry,
        IHttpClientFactory httpClientFactory)
    {
        _engine = engine;
        _stateService = stateService;
        _registry = registry;
        _httpClientFactory = httpClientFactory;
    }

    // POST /api/state/adapters
    [HttpPost("adapters")]
    public IActionResult SetAdapters([FromBody] AdapterSettingsRequest req)
    {
        if (req.ActuatorMode == "Http")
        {
            var client = new HttpClient { BaseAddress = new Uri("http://localhost:5055/") };
            _registry.SetActuatorAdapter(new HttpActuatorAdapter(client));
        }
        else
        {
            _registry.SetActuatorAdapter(new SimulatedActuatorAdapter());
        }

        if (req.NotificationMode == "Webhook")
        {
            var client = new HttpClient();
            _registry.SetNotificationAdapter(new WebhookNotificationAdapter(client, req.WebhookUrl ?? ""));
        }
        else
        {
            _registry.SetNotificationAdapter(new ConsoleNotificationAdapter());
        }

        return Ok("Adapters updated successfully.");
    }
    // POST /api/state/tick
    [HttpPost("tick")]
    public async Task<IActionResult> Tick([FromBody] RunStateTickRequest req, CancellationToken ct)
    {
        if (req == null || req.DeviceId <= 0)
            return BadRequest("Invalid deviceId.");

        var result = await _stateService.TickAsync(req.DeviceId, ct);
        return Ok(result);
    }

}
}
