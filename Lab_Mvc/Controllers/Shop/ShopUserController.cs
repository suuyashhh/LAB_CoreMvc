using Lab_Mvc.Interfaces.Shop;
using Microsoft.AspNetCore.Mvc;
using Models.Shop;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopUserController : ControllerBase
    {
        private readonly IShopUser _shopUser;

        public ShopUserController(IShopUser shopUser)
        {
            _shopUser = shopUser;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _shopUser.GetAll();
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
                var data = await _shopUser.GetById(userId);
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
        public async Task<IActionResult> Insert([FromBody] DTOShopLogin model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.USER_NAME) || string.IsNullOrEmpty(model.PASS) || string.IsNullOrEmpty(model.CONTACT))
                {
                    return BadRequest(new { success = false, message = "Username, password, and contact are required" });
                }

                var result = await _shopUser.Insert(model);
                return Ok(new { success = true, userId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOShopLogin model)
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

                var result = await _shopUser.Update(model);
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
                var result = await _shopUser.Delete(userId);
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
