using AzureEventGridSimulator.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;

namespace AzureEventGridSimulator.Controllers
{
    [Route("/api/health")]
    [ApiController]
    public class HealthController : SimulatorController
    {
        public HealthController(SimulatorSettings simulatorSettings) : base(simulatorSettings) { }

        [HttpGet]
        public OkResult Health()
        {
            return Ok();
        }
    }
}
