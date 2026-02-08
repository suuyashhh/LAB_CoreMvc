// DatePEController.cs
using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatePEController : ControllerBase
    {
        private readonly IDatePERepository _datePERepository;

        public DatePEController(IDatePERepository datePERepository)
        {
            _datePERepository = datePERepository;
        }

        [HttpPost("Summary")]
        public async Task<IActionResult> GetDateRangeSummary([FromBody] DatePEQueryDto query)
        {
            if (query == null || query.UserId <= 0)
                return BadRequest("Invalid query parameters");

            if (query.FromDate > query.ToDate)
                return BadRequest("From date cannot be later than To date");

            var result = await _datePERepository.GetDateRangeSummary(query);

            var model = new DatePEModel
            {
                DisplayDateRange = $"{result.FromDate:dd-MMM-yyyy} To {result.ToDate:dd-MMM-yyyy}",
                FromDate = result.FromDate.ToString("yyyy-MM-dd"),
                ToDate = result.ToDate.ToString("yyyy-MM-dd"),
                TotalBill = result.TotalBill,
                TotalExpense = result.TotalExpense,
                Profit = result.Profit,
                ProfitColor = result.Profit < 0 ? "red" : "green"
            };

            return Ok(model);
        }

        [HttpGet("DefaultRange/{userId:int}")]
        public async Task<IActionResult> GetDefaultDateRange(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID");

            // Get first and last day of current month as default
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var query = new DatePEQueryDto
            {
                UserId = userId,
                FromDate = firstDayOfMonth,
                ToDate = lastDayOfMonth
            };

            var result = await _datePERepository.GetDateRangeSummary(query);

            var model = new DatePEModel
            {
                DisplayDateRange = $"{firstDayOfMonth:dd-MMM-yyyy} To {lastDayOfMonth:dd-MMM-yyyy}",
                FromDate = firstDayOfMonth.ToString("yyyy-MM-dd"),
                ToDate = lastDayOfMonth.ToString("yyyy-MM-dd"),
                TotalBill = result.TotalBill,
                TotalExpense = result.TotalExpense,
                Profit = result.Profit,
                ProfitColor = result.Profit < 0 ? "red" : "green"
            };

            return Ok(model);
        }
    }
}