using Lab_Mvc.Interfaces.HaaS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.HaaS;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.HaaS
{
    [ApiController]
    [Route("api/haas/ecosystem")]
    public class HaaSEcosystemController : ControllerBase
    {
        private readonly IHaaSEcosystem _ecosystemRepo;

        public HaaSEcosystemController(IHaaSEcosystem ecosystemRepo)
        {
            _ecosystemRepo = ecosystemRepo;
        }

        /// <summary>GET /api/haas/ecosystem/demands</summary>
        [HttpGet("demands")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _ecosystemRepo.GetAllDemands();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>GET /api/haas/ecosystem/demands/by-season?seasonType=1</summary>
        [HttpGet("demands/by-season")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySeason([FromQuery] int seasonType)
        {
            try
            {
                var data = await _ecosystemRepo.GetDemandsBySeason(seasonType);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>GET /api/haas/ecosystem/demands/detail?ecosystemType=1&seasonType=1</summary>
        [HttpGet("demands/detail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetail([FromQuery] int ecosystemType, [FromQuery] int seasonType)
        {
            try
            {
                var data = await _ecosystemRepo.GetDemandByEcosystemAndSeason(ecosystemType, seasonType);
                if (data == null) return NotFound(new { message = "Demand not found." });
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>PUT /api/haas/ecosystem/demands – upsert demand value</summary>
        [HttpPut("demands")]
        [AllowAnonymous]
        public async Task<IActionResult> UpsertDemand([FromBody] DTOEcosystemDemand model)
        {
            try
            {
                var id = await _ecosystemRepo.UpsertDemand(model);
                return Ok(new { success = true, demandId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>POST /api/haas/ecosystem/seed – seed default demand data</summary>
        [HttpPost("seed")]
        [AllowAnonymous]
        public async Task<IActionResult> Seed()
        {
            try
            {
                await _ecosystemRepo.SeedDefaultDemands();
                return Ok(new { success = true, message = "Default demand data seeded for all 4 ecosystems × 4 seasons." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
