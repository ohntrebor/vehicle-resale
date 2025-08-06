using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

namespace VehicleResale.API.Controllers
{
    /// <summary>
    /// Controller para health checks
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] // Oculta do Swagger
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        /// <summary>
        /// Endpoint de health check
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            
            return report.Status == HealthStatus.Healthy 
                ? Ok(new { status = "Healthy", checks = report.Entries })
                : StatusCode(503, new { status = "Unhealthy", checks = report.Entries });
        }

        /// <summary>
        /// Endpoint simplificado para liveness probe
        /// </summary>
        [HttpGet("live")]
        public IActionResult GetLive()
        {
            return Ok(new { status = "alive" });
        }

        /// <summary>
        /// Endpoint para readiness probe
        /// </summary>
        [HttpGet("ready")]
        public async Task<IActionResult> GetReady()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            
            return report.Status == HealthStatus.Healthy 
                ? Ok(new { status = "ready" })
                : StatusCode(503, new { status = "not ready" });
        }
    }
}