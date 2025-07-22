using Lab_Mvc.Interfaces;
using Lab_Mvc.Repositries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IHome homeRepository;
        public HomeController(IHome homerepository)
        {
            this.homeRepository = homerepository;
        }


        [HttpGet("Home/{from_date},{to_date}")]
        public async Task<ActionResult> GetHomeById(string from_date, string to_date)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await homeRepository.GetHomeById(from_date, to_date));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
