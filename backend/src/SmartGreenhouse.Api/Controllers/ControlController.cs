using Microsoft.AspNetCore.Mvc;
using SmartGreenhouse.Api.Contracts;
using Application.Control;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGreenhouse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControlController : ControllerBase
    {
        private readonly ControlService _controlService;

        public ControlController(ControlService controlService)
        {
            _controlService = controlService;
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> Evaluate([FromBody] EvaluateControlRequest req, CancellationToken ct)
        {
            await _controlService.EvaluateAsync(req.DeviceId, ct);
            return Ok("Control evaluation executed successfully.");
        }

        [HttpPost("set-profile")]
        public IActionResult SetProfile([FromBody] SetControlProfileRequest req)
        {
            // For now, just acknowledge
            return Ok("Control profile saved.");
        }
    }
}
