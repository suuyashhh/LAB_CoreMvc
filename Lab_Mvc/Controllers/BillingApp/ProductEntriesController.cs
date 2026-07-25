using Lab_Mvc.Interfaces.BillingApp;
using Microsoft.AspNetCore.Mvc;
using Models.BillingApp;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.BillingApp
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductEntriesController : ControllerBase
    {
        private readonly IProductEntries _productEntriesRepository;

        public ProductEntriesController(IProductEntries productEntriesRepository)
        {
            _productEntriesRepository = productEntriesRepository;
        }

        [HttpGet("GetAllEntries")]
        public async Task<IActionResult> GetAllEntries()
        {
            try
            {
                var data = await _productEntriesRepository.GetAllEntries();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("SaveProductEntry")]
        public async Task<IActionResult> SaveProductEntry([FromBody] DTOProductEntries entry)
        {
            try
            {
                if (entry == null)
                {
                    return BadRequest(new { success = false, message = "Product entry data is required." });
                }

                if (string.IsNullOrWhiteSpace(entry.english_name))
                {
                    return BadRequest(new { success = false, message = "English name is required." });
                }

                if (string.IsNullOrWhiteSpace(entry.marathi_name))
                {
                    return BadRequest(new { success = false, message = "Marathi name is required." });
                }

                if (string.IsNullOrWhiteSpace(entry.quantity))
                {
                    return BadRequest(new { success = false, message = "Quantity is required." });
                }

                if (entry.price < 0)
                {
                    return BadRequest(new { success = false, message = "Price cannot be negative." });
                }

                bool success = await _productEntriesRepository.SaveProductEntry(entry);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("UpdateProductEntry")]
        public async Task<IActionResult> UpdateProductEntry([FromBody] DTOProductEntries entry)
        {
            try
            {
                if (entry == null)
                {
                    return BadRequest(new { success = false, message = "Product entry data is required." });
                }

                if (entry.product_id <= 0)
                {
                    return BadRequest(new { success = false, message = "Valid Product ID is required." });
                }

                if (string.IsNullOrWhiteSpace(entry.english_name))
                {
                    return BadRequest(new { success = false, message = "English name is required." });
                }

                if (string.IsNullOrWhiteSpace(entry.marathi_name))
                {
                    return BadRequest(new { success = false, message = "Marathi name is required." });
                }

                if (string.IsNullOrWhiteSpace(entry.quantity))
                {
                    return BadRequest(new { success = false, message = "Quantity is required." });
                }

                if (entry.price < 0)
                {
                    return BadRequest(new { success = false, message = "Price cannot be negative." });
                }

                bool success = await _productEntriesRepository.UpdateProductEntry(entry);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
