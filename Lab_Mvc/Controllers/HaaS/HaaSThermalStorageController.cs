using Lab_Mvc.Interfaces.HaaS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.HaaS;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.HaaS
{
    [ApiController]
    [Route("api/haas/storage")]
    public class HaaSThermalStorageController : ControllerBase
    {
        private readonly IHaaSThermalStorage _storageRepo;

        public HaaSThermalStorageController(IHaaSThermalStorage storageRepo)
        {
            _storageRepo = storageRepo;
        }

        /// <summary>GET /api/haas/storage/status</summary>
        [HttpGet("status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var data = await _storageRepo.GetCurrentStatus();
                if (data == null)
                    return Ok(new { message = "No storage record yet. Run an optimization first." });
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>GET /api/haas/storage/history?limit=50</summary>
        [HttpGet("history")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHistory([FromQuery] int limit = 50)
        {
            try
            {
                var data = await _storageRepo.GetHistory(limit);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>PUT /api/haas/storage/update</summary>
        [HttpPut("update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] DTOThermalStorageStatus model)
        {
            try
            {
                var id = await _storageRepo.UpdateStatus(model);
                return Ok(new { success = true, storageId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
