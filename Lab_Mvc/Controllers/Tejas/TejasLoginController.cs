using Lab_Mvc.Interfaces.Tejas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Tejas;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Tejas
{
    [ApiController]
    [Route("api/LoginTejas")]
    public class TejasLoginController : Controller
    {
        private readonly ITejasLogin _iTejasLogin;

        public TejasLoginController(ITejasLogin iTejasLogin)
        {
            _iTejasLogin = iTejasLogin;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOTejasLogin loginTejas)
        {
            try
            {
                var result = await _iTejasLogin.Login(loginTejas);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                return Ok(new
                {
                    userDetails = new
                    {
                        useR_ID = result.USER_ID,
                        useR_NAME = result.USER_NAME,
                        contact = result.CONTACT,
                        useR_IMG = result.USER_IMG,
                        role = result.ROLE
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong: " + ex.Message });
            }
        }
    }
}
