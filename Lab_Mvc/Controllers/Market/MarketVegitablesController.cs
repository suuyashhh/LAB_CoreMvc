using Lab_Mvc.Interfaces.Market;
using Lab_Mvc.Interfaces.Shop;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Lab_Mvc.Controllers.Market
{
    [ApiController]
    [Route("api/MarketVegitables")]
    public class MarketVegitablesController : Controller
    {
        private readonly IMarketVegitables _iMarketVegitables;

        public MarketVegitablesController(IMarketVegitables iMarketVegitables)
        {
            _iMarketVegitables = iMarketVegitables;
        }

        [HttpGet("GetVegetables")]
        public async Task<ActionResult> GetVegetables()
        {
            try
            {
                var result = await _iMarketVegitables.GetVegetables();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("SaveVegitable")]
        public async Task<ActionResult> SaveVegitable(string name)
        {
            try
            {
                var result = await _iMarketVegitables.SaveVegitable(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
