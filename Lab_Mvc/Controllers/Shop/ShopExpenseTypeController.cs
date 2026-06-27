using Lab_Mvc.Interfaces.Shop;
using Microsoft.AspNetCore.Mvc;
using Models.Shop;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopExpenseTypeController : ControllerBase
    {
        private readonly IShopExpenseType _shopExpenseType;

        public ShopExpenseTypeController(IShopExpenseType shopExpenseType)
        {
            _shopExpenseType = shopExpenseType;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _shopExpenseType.GetAll();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int exId)
        {
            try
            {
                var data = await _shopExpenseType.GetById(exId);
                if (data == null)
                {
                    return NotFound(new { message = "Expense type not found" });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] DTOShopExpenseType model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.NAME))
                {
                    return BadRequest(new { success = false, message = "Name is required" });
                }

                var result = await _shopExpenseType.Insert(model);
                return Ok(new { success = true, exId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOShopExpenseType model)
        {
            try
            {
                if (model.EX_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Valid Expense Type ID is required" });
                }
                if (string.IsNullOrEmpty(model.NAME))
                {
                    return BadRequest(new { success = false, message = "Name is required" });
                }

                var result = await _shopExpenseType.Update(model);
                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Expense type not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int exId)
        {
            try
            {
                var result = await _shopExpenseType.Delete(exId);
                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Expense type not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
