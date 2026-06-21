using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopFileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ShopFileUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file uploaded" });

                // Validate file size (5MB limit)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    return BadRequest(new { success = false, message = "Invalid file type. Allowed: JPG, PNG, GIF, WebP" });

                // Create unique file name
                var fileName = $"{Guid.NewGuid():N}{extension}";

                // Define upload path - ShopImgs folder at root level
                var uploadPath = Path.Combine(_environment.ContentRootPath, "ShopImgs");

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Full file path
                var filePath = Path.Combine(uploadPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path for storage in database
                var relativePath = $"/ShopImgs/{fileName}";

                return Ok(new
                {
                    success = true,
                    message = "File uploaded successfully",
                    fileName = fileName,
                    filePath = relativePath,
                    fullUrl = $"{Request.Scheme}://{Request.Host}{relativePath}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error uploading file: {ex.Message}" });
            }
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return BadRequest(new { success = false, message = "File name is required" });

                // Define upload path - ShopImgs folder at root level
                var uploadPath = Path.Combine(_environment.ContentRootPath, "ShopImgs");
                var filePath = Path.Combine(uploadPath, fileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { success = false, message = "File not found" });

                System.IO.File.Delete(filePath);

                return Ok(new { success = true, message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error deleting file: {ex.Message}" });
            }
        }

        [HttpGet("GetImage/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return BadRequest("File name is required");

                var uploadPath = Path.Combine(_environment.ContentRootPath, "ShopImgs");
                var filePath = Path.Combine(uploadPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Image not found");
                }

                // Determine content type
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving image: {ex.Message}");
            }
        }
    }
}
