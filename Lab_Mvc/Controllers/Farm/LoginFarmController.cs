using Lab_Mvc.Interfaces.DairyFarm;
using Lab_Mvc.Interfaces.Farm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;
using Models.Farm;

namespace Lab_Mvc.Controllers.Farm
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginFarmController : Controller
    {
        private readonly ILoginFarm _iLoginFarm;

        public LoginFarmController(ILoginFarm iLoginFarm)
        {
            _iLoginFarm = iLoginFarm;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginFarm([FromBody] DTOLoginFarm loginFarm)
        {
            try
            {
                var result = await _iLoginFarm.LoginFarm(loginFarm);

                if (result == null)
                    return Unauthorized("Invalid credentials");

                return Ok(new
                {
                    result.USER_ID,
                    result.USER_NAME
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }
    }
}
