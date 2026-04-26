using Microsoft.AspNetCore.Mvc;
using Models;
using SmartParking.Interfaces;

namespace SmartParking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingLoginController : ControllerBase
    {
        private readonly IParkingLogin _parkingLogin;

        public ParkingLoginController(IParkingLogin parkingLogin)
        {
            _parkingLogin = parkingLogin;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOParkingLogin loginRequest)
        {
            var result = await _parkingLogin.Login(loginRequest.PHONE ?? "", loginRequest.PASS ?? "");
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid phone or password" });
            }
            return Ok(result);
        }
    }
}
