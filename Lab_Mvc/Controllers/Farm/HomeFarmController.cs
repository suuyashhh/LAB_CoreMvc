using Lab_Mvc.Interfaces.Farm;
using Microsoft.AspNetCore.Mvc;
using Models.Farm;

namespace Lab_Mvc.Controllers.Farm
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeFarmController : ControllerBase
    {
        private readonly IHomeFarm _iHomeFarm;

        public HomeFarmController(IHomeFarm iHomeFarm)
        {
            _iHomeFarm = iHomeFarm;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string userId)
        {
            try
            {
                var data = await _iHomeFarm.GetAll(userId);

                // Convert relative paths to full URLs
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                foreach (var farm in data)
                {
                    if (!string.IsNullOrEmpty(farm.IMAGE) &&
                        !farm.IMAGE.StartsWith("http") &&
                        !farm.IMAGE.StartsWith("data:"))
                    {
                        farm.IMAGE = $"{baseUrl}{farm.IMAGE}";
                    }
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] DTOHomeFarm model)
        {
            try
            {
                var result = await _iHomeFarm.Insert(model);
                return Ok(new { success = true, farmId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOHomeFarm model)
        {
            try
            {
                var result = await _iHomeFarm.Update(model);
                return Ok(new { success = result > 0, affectedRows = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int farmId, string userId)
        {
            try
            {
                var result = await _iHomeFarm.Delete(farmId, userId);
                return Ok(new { success = result > 0, affectedRows = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}