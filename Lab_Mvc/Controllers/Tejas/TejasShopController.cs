using Lab_Mvc.Interfaces.Tejas;
using Microsoft.AspNetCore.Mvc;
using Models.Tejas;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Tejas
{
    [ApiController]
    [Route("api/[controller]")]
    public class TejasShopController : ControllerBase
    {
        private readonly ITejasShop _tejasShop;

        public TejasShopController(ITejasShop tejasShop)
        {
            _tejasShop = tejasShop;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _tejasShop.GetAll();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long shopId)
        {
            try
            {
                var data = await _tejasShop.GetById(shopId);
                if (data == null)
                {
                    return NotFound(new { message = "Shop not found" });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] DTOTejasShop model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.SHOP_NAME))
                {
                    return BadRequest(new { success = false, message = "Shop name is required" });
                }

                var result = await _tejasShop.Insert(model);
                return Ok(new { success = true, shopId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOTejasShop model)
        {
            try
            {
                if (model.TEJAS_SHOPES_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Valid Shop ID is required" });
                }
                if (string.IsNullOrEmpty(model.SHOP_NAME))
                {
                    return BadRequest(new { success = false, message = "Shop name is required" });
                }

                var result = await _tejasShop.Update(model);
                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Shop not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long shopId)
        {
            try
            {
                var result = await _tejasShop.Delete(shopId);
                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Shop not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
