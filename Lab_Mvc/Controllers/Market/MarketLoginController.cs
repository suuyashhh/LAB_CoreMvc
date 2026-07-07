using Lab_Mvc.Interfaces.Market;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers.Market
{
    [ApiController]           //This class is an API controller
    [Route("api/[controller]")]  //defines the URL for the controller.
    public class MarketLoginController : Controller
    {
        public readonly IMarketLogin _marketLogin;   //The reference cannot be changed after it is initialized.

        public MarketLoginController(IMarketLogin marketLogin)
        {
            _marketLogin = marketLogin;
        }


        [HttpGet("LoginCheck")]
        public async Task<ActionResult> LoginCheck(string contact, string pass)
        {
            try
            {
                var result = await _marketLogin.LoginCheck(contact, pass);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
