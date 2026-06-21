using Lab_Mvc.Interfaces.Shop;
using Microsoft.AspNetCore.Mvc;
using Models.Shop;
using System.Threading.Tasks;
using System;

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
        public async Task<IActionResult> Login([FromBody] DTOShopLogin loginShop)
        {
            try
            {
                var result = await _iShopLogin.Login(loginShop);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong: " + ex.Message });
            }
        }
    }
}
