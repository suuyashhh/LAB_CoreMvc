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

        [HttpPost("GetUserDetails/{user_id}")]
        public async Task<IActionResult> GetUserDetails(string user_id)
        {
            try
            {
                var result = await _iLoginFarm.GetUserDetails(user_id);

                if (result == null)
                    return Unauthorized("Invalid credentials");

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpPost("UpdatetUserDetails")]
        public async Task<IActionResult> UpdatetUserDetails([FromBody] DTOLoginFarm USERDETAILS)
        {
            try
            {
                var result = await _iLoginFarm.UpdatetUserDetails(USERDETAILS);

                if (result == null)
                    return NotFound("User not found or update failed");

                return Ok(new
                {
                    result.USER_ID,
                    result.USER_NAME,
                    result.CONTACT,
                    result.FROM_DATE,
                    result.TO_DATE
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong: " + ex.Message);
            }
        }
    }
}
