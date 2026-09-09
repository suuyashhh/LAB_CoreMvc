using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using Models.Fab;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabLoginController : ControllerBase
    {
        private readonly IFabLoginRepository _loginRepository;

        public FabLoginController(IFabLoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] DTOFabLogin loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password) || string.IsNullOrEmpty(loginDto.Type))
                {
                    return BadRequest("Username, Password, and Type are required.");
                }

                DTOFabLogin? result;
                if (loginDto.Type.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _loginRepository.LoginAdmin(loginDto.Username, loginDto.Password);
                }
                else
                {
                    result = await _loginRepository.LoginHelper(loginDto.Username, loginDto.Password);
                }

                if (result == null)
                {
                    return Unauthorized("Invalid credentials.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("RegisterHelper")]
        public async Task<IActionResult> RegisterHelper([FromBody] DTOFabUsers helperDto)
        {
            try
            {
                if (helperDto == null || string.IsNullOrEmpty(helperDto.User_name) || string.IsNullOrEmpty(helperDto.User_contact) || string.IsNullOrEmpty(helperDto.User_pass))
                {
                    return BadRequest("Name, Contact and Password are required.");
                }

                bool result = await _loginRepository.RegisterHelper(helperDto);
                if (!result)
                {
                    return Conflict("This contact is already registered.");
                }

                return Ok(new { success = true, message = "Helper registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
