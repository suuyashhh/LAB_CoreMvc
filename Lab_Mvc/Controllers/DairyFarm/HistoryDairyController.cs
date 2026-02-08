using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lab_Mvc.Repositries.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryDairyController : Controller
    {
        private readonly IHistoryDairy _historyDairy;

        public HistoryDairyController(IHistoryDairy historyDairy)
        {
            _historyDairy = historyDairy;
        }

        // ================= GET ALL HISTORY =================
        [HttpGet("History/{userId}")]
        public async Task<IActionResult> GetAllHistory(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var history = await _historyDairy.GetAllHistory(userId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving history", error = ex.Message });
            }
        }

        // ================= GET HISTORY IMAGE =================
        [HttpGet("GetHistoryImage/{expenseId}/{expenseType}")]
        public async Task<IActionResult> GetHistoryImage(long expenseId, string expenseType)
        {
            try
            {
                if (expenseId <= 0)
                    return BadRequest(new { message = "Invalid expense ID" });

                if (string.IsNullOrEmpty(expenseType))
                    return BadRequest(new { message = "Expense type is required" });

                var result = await _historyDairy.GetHistoryImageById(expenseId, expenseType);

                if (result == null)
                    return NotFound(new { message = "Image not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving image", error = ex.Message });
            }
        }
    }
}