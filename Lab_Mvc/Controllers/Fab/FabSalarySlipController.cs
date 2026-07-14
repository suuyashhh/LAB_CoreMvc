using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using Models.Fab;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabSalarySlipController : ControllerBase
    {
        private readonly IFabSalarySlipRepository _salarySlipRepository;

        public FabSalarySlipController(IFabSalarySlipRepository salarySlipRepository)
        {
            _salarySlipRepository = salarySlipRepository;
        }

        [HttpPost("SalarySlip")]
        public async Task<IActionResult> SaveSalarySlip([FromBody] DTOSalarySlip salarySlip)
        {
            try
            {
                bool success = await _salarySlipRepository.SaveSalarySlip(salarySlip);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SalarySlips")]
        public async Task<IActionResult> GetSalarySlipsHistory()
        {
            try
            {
                var history = await _salarySlipRepository.GetSalarySlipsHistory();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Support both routes to ensure frontend compatibility
        [HttpGet("SalarySlips/Helper/{userId}")]
        [HttpGet("Helper/{userId}/SalarySlips")]
        public async Task<IActionResult> GetHelperSalaryHistory(int userId)
        {
            try
            {
                var history = await _salarySlipRepository.GetHelperSalaryHistory(userId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SalarySlips/Range")]
        public async Task<IActionResult> GetSalarySlipsByRange([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var slips = await _salarySlipRepository.GetSalarySlipsByDateRange(from, to);
                return Ok(slips);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
