using SmartParking.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.SmartParking;

namespace SmartParking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingRegistrationController : Controller
    {
        private readonly IParkingRegistration _IParkingRegistration;

        public ParkingRegistrationController(IParkingRegistration IParkingRegistration)
        {
            _IParkingRegistration = IParkingRegistration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] DTOParkingRegistration registration)
        {
            var result = await _IParkingRegistration.Register(registration);
            if (result > 0)
            {
                return Ok(new { message = "Registration successful" });
            }
            return BadRequest(new { message = "Registration failed" });
        }
    }
}
