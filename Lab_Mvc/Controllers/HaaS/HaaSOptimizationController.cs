using Lab_Mvc.Interfaces.HaaS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.HaaS;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.HaaS
{
    [ApiController]
    [Route("api/haas/optimize")]
    public class HaaSOptimizationController : ControllerBase
    {
        private readonly IHaaSOptimization _optimizationRepo;

        public HaaSOptimizationController(IHaaSOptimization optimizationRepo)
        {
            _optimizationRepo = optimizationRepo;
        }

        /// <summary>
        /// POST /api/haas/optimize/run
        /// Triggers the optimization engine. Simulates data center metrics,
        /// fetches ecosystem demands, and allocates heat by priority.
        /// </summary>
        [HttpPost("run")]
        [AllowAnonymous]
        public async Task<IActionResult> RunOptimization([FromBody] DTOOptimizationRequest request)
        {
            try
            {
                if (request == null)
                {
                    // Default request: Winter season, 5°C outside
                    request = new DTOOptimizationRequest
                    {
                        SeasonType                   = (int)SeasonType.Winter,
                        OutsideTemperatureCelsius    = 5,
                        ForceStorageDischarge        = false
                    };
                }

                var result = await _optimizationRepo.RunOptimization(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Optimization failed: {ex.Message}" });
            }
        }

        /// <summary>GET /api/haas/optimize/logs?from=&to=&limit=100</summary>
        [HttpGet("logs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLogs(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int limit = 100)
        {
            try
            {
                var logs = await _optimizationRepo.GetOptimizationLogs(from, to, limit);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }
    }
}
