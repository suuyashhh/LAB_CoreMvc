using Lab_Mvc.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using Models.Admin;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainAdminController : ControllerBase
    {
        private readonly IMainAdminRepository _repository;

        public MainAdminController(IMainAdminRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOMainAdminLogin loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "Username and Password are required." });
            }

            var admin = await _repository.LoginMainAdmin(loginDto);

            if (admin == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(new { 
                message = "Login successful", 
                adminId = admin.Id,
                username = admin.Username 
            });
        }
    }
}
