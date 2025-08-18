using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceController : Controller
    {
        private readonly IFinance financeRepository;
        public FinanceController(IFinance financerepository)
        {
            this.financeRepository = financerepository;
        }


        [HttpGet("Finance/{from_date},{to_date}")]
        public async Task<ActionResult> GetFinanceById(string from_date, string to_date, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await financeRepository.GetFinanceById(from_date, to_date, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
