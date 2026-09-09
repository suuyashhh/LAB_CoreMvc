using Lab_Mvc.Interfaces.Tejas;
using Microsoft.AspNetCore.Mvc;
using Models.Tejas;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Tejas
{
    [ApiController]
    [Route("api/[controller]")]
    public class TejasExpenseTypeController : ControllerBase
    {
        private readonly ITejasExpenseType _tejasExpenseType;

        public TejasExpenseTypeController(ITejasExpenseType tejasExpenseType)
        {
            _tejasExpenseType = tejasExpenseType;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] long? shopId = null)
        {
            try
            {
                var data = await _tejasExpenseType.GetAll(shopId);
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
                var data = await _tejasExpenseType.GetById(exId);
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
        public async Task<IActionResult> Insert([FromBody] DTOTejasExpenseType model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.NAME))
                {
                    return BadRequest(new { success = false, message = "Name is required" });
                }

                var result = await _tejasExpenseType.Insert(model);
                return Ok(new { success = true, exId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOTejasExpenseType model)
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

                var result = await _tejasExpenseType.Update(model);
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
                var result = await _tejasExpenseType.Delete(exId);
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
