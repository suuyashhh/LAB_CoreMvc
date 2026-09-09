using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabPESummaryController : ControllerBase
    {
        private readonly IFabPESummaryRepository _peSummaryRepository;

        public FabPESummaryController(IFabPESummaryRepository peSummaryRepository)
        {
            _peSummaryRepository = peSummaryRepository;
        }

        [HttpGet("PE/Summary")]
        [HttpGet("ProfitExpense/Range")]
        public async Task<IActionResult> GetPESummary([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var summary = await _peSummaryRepository.GetProfitExpenseSummaryForDateRange(from, to);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("PE/Monthly")]
        [HttpGet("ProfitExpense/Monthly")]
        public async Task<IActionResult> GetPEMonthly()
        {
            try
            {
                var monthly = await _peSummaryRepository.GetMonthlyProfitExpenseSummary();
                return Ok(monthly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
