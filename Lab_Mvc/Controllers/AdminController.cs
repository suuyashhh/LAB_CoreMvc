using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly IAdmin _adminRepository;

        public AdminController(IAdmin adminrepository)
        {
            this._adminRepository = adminrepository;
        }

        [HttpGet("Companies")]
        public async Task<ActionResult> Companies()
        {
            try
            {
                return Ok(await _adminRepository.GetCompanies());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
