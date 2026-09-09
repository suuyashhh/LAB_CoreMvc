using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;

namespace Lab_Mvc.Controllers.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginDairFarmController : Controller
    {
        private readonly ILoginDairyFarm _loginDairyFarm;

        public LoginDairFarmController(ILoginDairyFarm loginDairyFarm)
        {
            _loginDairyFarm = loginDairyFarm;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginDairyFarm([FromBody] DTOLoginDairyFarm loginDairyFarm)
        {
            try
            {
                var result = await _loginDairyFarm.LoginDairyFarm(loginDairyFarm);

                if (result == null)
                    return Unauthorized("Invalid credentials");

                return Ok(new
                {
                    result.user_id,
                    result.user_name,
                    result.contact
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterDairyFarm([FromBody] DTOLoginDairyFarm registerDairyFarm)
        {
            try
            {
                if (string.IsNullOrEmpty(registerDairyFarm.contact) || string.IsNullOrEmpty(registerDairyFarm.password) || string.IsNullOrEmpty(registerDairyFarm.user_name) || string.IsNullOrEmpty(registerDairyFarm.email))
                {
                    return BadRequest("All fields (user name, contact, email, password) are required");
                }

                var result = await _loginDairyFarm.RegisterDairyFarm(registerDairyFarm);

                if (!result)
                    return BadRequest("User with this contact already exists or registration failed");

                return Ok(new { message = "Registration successful" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }
    }
}
