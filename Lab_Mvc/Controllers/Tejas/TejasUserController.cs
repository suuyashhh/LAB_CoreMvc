using Lab_Mvc.Interfaces.Tejas;
using Microsoft.AspNetCore.Mvc;
using Models.Tejas;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Tejas
{
    [ApiController]
    [Route("api/[controller]")]
    public class TejasUserController : ControllerBase
    {
        private readonly ITejasUser _tejasUser;

        public TejasUserController(ITejasUser tejasUser)
        {
            _tejasUser = tejasUser;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _tejasUser.GetAll();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long userId)
        {
            try
            {
                var data = await _tejasUser.GetById(userId);
                if (data == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] DTOTejasLogin model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.USER_NAME) || string.IsNullOrEmpty(model.PASS) || string.IsNullOrEmpty(model.CONTACT))
                {
                    return BadRequest(new { success = false, message = "Username, password, and contact are required" });
                }

                var result = await _tejasUser.Insert(model);
                return Ok(new { success = true, userId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOTejasLogin model)
        {
            try
            {
                if (model.USER_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Valid User ID is required" });
                }
                if (string.IsNullOrEmpty(model.USER_NAME) || string.IsNullOrEmpty(model.PASS) || string.IsNullOrEmpty(model.CONTACT))
                {
                    return BadRequest(new { success = false, message = "Username, password, and contact are required" });
                }

                var result = await _tejasUser.Update(model);
                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long userId)
        {
            try
            {
                var result = await _tejasUser.Delete(userId);
                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
