using Lab_Mvc.Interfaces.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Shop;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Shop
{
    [ApiController]
    [Route("api/LoginShop")]
    public class ShopLoginController : Controller
    {
        private readonly IShopLogin _iShopLogin;

        public ShopLoginController(IShopLogin iShopLogin)
        {
            _iShopLogin = iShopLogin;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOShopLogin loginShop)
        {
            try
            {
                var result = await _iShopLogin.Login(loginShop);

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
                        useR_IMG = result.USER_IMG
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
