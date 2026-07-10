using System.Threading.Tasks;
using Lab_Mvc.Interfaces.Market;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers.Market
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/dashboard")] // Fallback explicit route mapping
    public class MarketDashboardController : ControllerBase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public MarketDashboardController(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _purchaseRepository.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}
