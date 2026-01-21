// MonthlyPEController.cs
using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonthlyPEController : ControllerBase
    {
        private readonly IMonthlyPERepository _monthlyPERepository;

        public MonthlyPEController(IMonthlyPERepository monthlyPERepository)
        {
            _monthlyPERepository = monthlyPERepository;
        }

        [HttpGet("Summary/{userId:int}")]
        public async Task<IActionResult> GetMonthlySummary(int userId)
        {
            var monthlyData = await _monthlyPERepository.GetMonthlySummary(userId);

            if (!monthlyData.Any())
                return Ok(new List<MonthlyPEModel>());

            var models = monthlyData.Select(dto => new MonthlyPEModel
            {
                DisplayMonth = $"{dto.MonthName} {dto.YearValue}",
                TotalBill = dto.TotalBill,
                TotalExpense = dto.TotalExpense,
                Profit = dto.Profit,
                ProfitColor = dto.Profit < 0 ? "red" : "green"
            }).ToList();

            return Ok(models);
        }

        // Optional: Get raw data (DTOs) if needed
        [HttpGet("RawSummary/{userId:int}")]
        public async Task<IActionResult> GetMonthlySummaryRaw(int userId)
        {
            var monthlyData = await _monthlyPERepository.GetMonthlySummary(userId);
            return Ok(monthlyData);
        }
    }
}