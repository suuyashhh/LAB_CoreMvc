using Lab_Mvc.Interfaces.HaaS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.HaaS;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.HaaS
{
    [ApiController]
    [Route("api/haas/datacenter")]
    public class HaaSDataCenterController : ControllerBase
    {
        private readonly IHaaSDataCenter _dataCenterRepo;

        public HaaSDataCenterController(IHaaSDataCenter dataCenterRepo)
        {
            _dataCenterRepo = dataCenterRepo;
        }

        /// <summary>GET /api/haas/datacenter/latest</summary>
        [HttpGet("latest")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatest()
        {
            try
            {
                var data = await _dataCenterRepo.GetLatestMetrics();
                if (data == null)
                    return Ok(new { message = "No data yet. Use /simulate to generate first entry." });
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>GET /api/haas/datacenter/history?from=&to=</summary>
        [HttpGet("history")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHistory([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var data = await _dataCenterRepo.GetMetricsHistory(from, to);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>POST /api/haas/datacenter/simulate</summary>
        [HttpPost("simulate")]
        [AllowAnonymous]
        public async Task<IActionResult> Simulate([FromQuery] double outsideTemp = 15, [FromQuery] int season = 1)
        {
            try
            {
                var data = await _dataCenterRepo.SimulateMetrics(outsideTemp, season);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>POST /api/haas/datacenter/insert</summary>
        [HttpPost("insert")]
        [AllowAnonymous]
        public async Task<IActionResult> Insert([FromBody] DTODataCenterMetrics model)
        {
            try
            {
                var id = await _dataCenterRepo.InsertMetrics(model);
                return Ok(new { success = true, metricsId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
