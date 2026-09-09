using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabAdvanceController : ControllerBase
    {
        private readonly IFabAdvanceRepository _advanceRepository;

        public FabAdvanceController(IFabAdvanceRepository advanceRepository)
        {
            _advanceRepository = advanceRepository;
        }

        [HttpGet("Advances")]
        public async Task<IActionResult> GetAdvances()
        {
            try
            {
                var advances = await _advanceRepository.GetAdvances();
                return Ok(advances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Helper/Advances/{userId}")]
        public async Task<IActionResult> GetHelperAdvanceHistory(int userId)
        {
            try
            {
                var history = await _advanceRepository.GetHelperAdvanceHistory(userId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
