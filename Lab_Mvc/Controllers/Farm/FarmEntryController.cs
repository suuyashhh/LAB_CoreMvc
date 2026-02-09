using Lab_Mvc.Interfaces.Farm;
using Microsoft.AspNetCore.Mvc;
using Models.Farm;

namespace Lab_Mvc.Controllers.Farm
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmEntryController : ControllerBase
    {
        private readonly IFarmEntry _IFarmEntry;

        public FarmEntryController(IFarmEntry IFarmEntry)
        {
            _IFarmEntry = IFarmEntry;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(long farmId, long userId,string entryTypeName)
        {
            try
            {
                var data = await _IFarmEntry.GetAll(farmId, userId, entryTypeName);

                // Convert relative paths to full URLs for all 4 images
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                foreach (var entry in data)
                {
                    entry.IMAGE1 = ConvertToFullUrl(entry.IMAGE1, baseUrl);
                    entry.IMAGE2 = ConvertToFullUrl(entry.IMAGE2, baseUrl);
                    entry.IMAGE3 = ConvertToFullUrl(entry.IMAGE3, baseUrl);
                    entry.IMAGE4 = ConvertToFullUrl(entry.IMAGE4, baseUrl);
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long farmEntryId, long farmId, long userId)
        {
            try
            {
                var data = await _IFarmEntry.GetById(farmEntryId, farmId, userId);

                if (data == null)
                {
                    return NotFound(new { message = "Farm entry not found" });
                }

                // Convert relative paths to full URLs
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                data.IMAGE1 = ConvertToFullUrl(data.IMAGE1, baseUrl);
                data.IMAGE2 = ConvertToFullUrl(data.IMAGE2, baseUrl);
                data.IMAGE3 = ConvertToFullUrl(data.IMAGE3, baseUrl);
                data.IMAGE4 = ConvertToFullUrl(data.IMAGE4, baseUrl);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] DTOFarmEntry model)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(model.ENTRY_TYPE))
                {
                    return BadRequest(new { success = false, message = "Entry type is required" });
                }

                if (model.FARM_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Farm ID is required" });
                }

                if (model.USER_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "User ID is required" });
                }

                var result = await _IFarmEntry.Insert(model);
                return Ok(new { success = true, farmEntryId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOFarmEntry model)
        {
            try
            {
                // Validate required fields
                if (model.FARM_ENTRY_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Farm Entry ID is required" });
                }

                if (model.FARM_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Farm ID is required" });
                }

                if (model.USER_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "User ID is required" });
                }

                var result = await _IFarmEntry.Update(model);

                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Farm entry not found or not authorized" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long farmEntryId, long farmId, long userId)
        {
            try
            {
                // First, get the farm entry to find its image paths
                var entryToDelete = await _IFarmEntry.GetById(farmEntryId, farmId, userId);

                if (entryToDelete != null)
                {
                    // Delete all associated images
                    DeleteImageFile(entryToDelete.IMAGE1);
                    DeleteImageFile(entryToDelete.IMAGE2);
                    DeleteImageFile(entryToDelete.IMAGE3);
                    DeleteImageFile(entryToDelete.IMAGE4);
                }

                // Delete from database
                var result = await _IFarmEntry.Delete(farmEntryId, farmId, userId);

                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Farm entry not found or not authorized" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #region Helper Methods

        private string ConvertToFullUrl(string imagePath, string baseUrl)
        {
            if (string.IsNullOrEmpty(imagePath))
                return imagePath;

            if (imagePath.StartsWith("http") || imagePath.StartsWith("data:"))
                return imagePath;

            return $"{baseUrl}{imagePath}";
        }

        private void DeleteImageFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            try
            {
                // Extract filename from image path
                var fileName = Path.GetFileName(imagePath);
                if (!string.IsNullOrEmpty(fileName))
                {
                    // Delete the image file
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "FarmImgs");
                    var filePath = Path.Combine(uploadPath, fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - we still want to delete the database record
                Console.WriteLine($"Error deleting image file: {ex.Message}");
            }
        }

        #endregion
    }
}