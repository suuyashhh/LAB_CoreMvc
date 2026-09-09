using Lab_Mvc.Interfaces.Tejas;
using Microsoft.AspNetCore.Mvc;
using Models.Tejas;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Tejas
{
    [ApiController]
    [Route("api/[controller]")]
    public class TejasEntryController : ControllerBase
    {
        private readonly ITejasEntry _ITejasEntry;

        public TejasEntryController(ITejasEntry ITejasEntry)
        {
            _ITejasEntry = ITejasEntry;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(long userId, bool isPaid, [FromQuery] long? shopId = null)
        {
            try
            {
                var data = await _ITejasEntry.GetAll(userId, isPaid, shopId);

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

        [HttpGet("GetAllTypesEntrys")]
        public async Task<IActionResult> GetAllTypesEntrys(long userId, DateTime? fromDate = null, DateTime? toDate = null, [FromQuery] long? shopId = null)
        {
            try
            {
                if (fromDate.HasValue)
                {
                    fromDate = fromDate.Value.Date;
                }
                if (toDate.HasValue)
                {
                    toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                }

                var data = await _ITejasEntry.GetAllTypesEntrys(userId, fromDate, toDate, shopId);

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
        public async Task<IActionResult> GetById(long tejasEntryId, long userId)
        {
            try
            {
                var data = await _ITejasEntry.GetById(tejasEntryId, userId);

                if (data == null)
                {
                    return NotFound(new { message = "Tejas entry not found" });
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
        public async Task<IActionResult> Insert([FromBody] DTOTejasEntry model)
        {
            try
            {
                if (model.USER_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "User ID is required" });
                }

                var result = await _ITejasEntry.Insert(model);
                return Ok(new { success = true, tejasEntryId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOTejasEntry model)
        {
            try
            {
                // Validate required fields
                if (model.TEJAS_ENTRY_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Tejas Entry ID is required" });
                }

                if (model.USER_ID <= 0)
                {
                    return BadRequest(new { success = false, message = "User ID is required" });
                }

                var result = await _ITejasEntry.Update(model);

                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Tejas entry not found or not authorized" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long tejasEntryId, long userId)
        {
            try
            {
                // First, get the tejas entry to find its image paths
                var entryToDelete = await _ITejasEntry.GetById(tejasEntryId, userId);

                if (entryToDelete != null)
                {
                    // Delete all associated images
                    DeleteImageFile(entryToDelete.IMAGE1);
                    DeleteImageFile(entryToDelete.IMAGE2);
                    DeleteImageFile(entryToDelete.IMAGE3);
                    DeleteImageFile(entryToDelete.IMAGE4);
                }

                // Delete from database
                var result = await _ITejasEntry.Delete(tejasEntryId, userId);

                if (result > 0)
                {
                    return Ok(new { success = true, affectedRows = result });
                }
                else
                {
                    return NotFound(new { success = false, message = "Tejas entry not found or not authorized" });
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
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "TejasImgs");
                    var filePath = Path.Combine(uploadPath, fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image file: {ex.Message}");
            }
        }

        #endregion
    }
}
