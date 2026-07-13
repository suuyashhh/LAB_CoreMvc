using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using Models.Fab;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabUsersController : ControllerBase
    {
        private readonly IFabUsersRepository _usersRepository;

        public FabUsersController(IFabUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet("Helpers")]
        public async Task<IActionResult> GetAllHelpers()
        {
            try
            {
                var helpers = await _usersRepository.GetAllHelpers();
                return Ok(helpers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Helper")]
        public async Task<IActionResult> UpdateHelper([FromBody] DTOFabUsers helperDto)
        {
            try
            {
                if (helperDto == null || !helperDto.User_id.HasValue)
                {
                    return BadRequest("Helper details with valid ID are required.");
                }

                bool success = await _usersRepository.UpdateHelper(helperDto);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Helper/{userId}")]
        public async Task<IActionResult> DeleteHelper(int userId)
        {
            try
            {
                bool success = await _usersRepository.DeleteHelper(userId);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
