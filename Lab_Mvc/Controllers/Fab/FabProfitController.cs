using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using Models.Fab;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabProfitController : ControllerBase
    {
        private readonly IFabProfitRepository _profitRepository;

        public FabProfitController(IFabProfitRepository profitRepository)
        {
            _profitRepository = profitRepository;
        }

        [HttpPost("Profit")]
        public async Task<IActionResult> InsertProfit([FromBody] DTOFabProfit profit)
        {
            try
            {
                bool success = await _profitRepository.InsertProfit(profit);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Profit")]
        public async Task<IActionResult> UpdateProfit([FromBody] DTOFabProfit profit)
        {
            try
            {
                bool success = await _profitRepository.UpdateProfit(profit);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Profit/{profitId}")]
        public async Task<IActionResult> DeleteProfit(int profitId)
        {
            try
            {
                bool success = await _profitRepository.DeleteProfit(profitId);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Profits")]
        [HttpGet("Profits/Range")]
        public async Task<IActionResult> GetProfits([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var profits = await _profitRepository.GetProfitsByDateRange(from, to);
                return Ok(profits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
