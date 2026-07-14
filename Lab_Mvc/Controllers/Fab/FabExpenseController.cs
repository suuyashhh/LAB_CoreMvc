using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using Models.Fab;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabExpenseController : ControllerBase
    {
        private readonly IFabExpenseRepository _expenseRepository;

        public FabExpenseController(IFabExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        [HttpPost("Expense")]
        public async Task<IActionResult> InsertExpense([FromBody] DTOFabExpanse expense)
        {
            try
            {
                bool success = await _expenseRepository.InsertExpense(expense);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Expense")]
        public async Task<IActionResult> UpdateExpense([FromBody] DTOFabExpanse expense)
        {
            try
            {
                bool success = await _expenseRepository.UpdateExpense(expense);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Expense/{expenseId}")]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
            try
            {
                bool success = await _expenseRepository.DeleteExpense(expenseId);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Expenses")]
        [HttpGet("Expenses/Range")]
        public async Task<IActionResult> GetExpenses([FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] bool onlyGeneral)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var expenses = await _expenseRepository.GetExpensesByDateRange(from, to, onlyGeneral);
                return Ok(expenses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
