using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SmartParking.Interfaces;
using System.Security.Claims;

namespace SmartParking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingProviderController : ControllerBase
    {
        private readonly IParkingProvider _iParkingProvider;
        private readonly IWebHostEnvironment _environment;

        public ParkingProviderController(IParkingProvider iParkingProvider, IWebHostEnvironment environment)
        {
            _iParkingProvider = iParkingProvider;
            _environment = environment;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file uploaded" });

                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    return BadRequest(new { success = false, message = "Invalid file type. Allowed: JPG, PNG, GIF, WebP" });

                var fileName = $"{Guid.NewGuid():N}{extension}";
                var uploadPath = Path.Combine(_environment.ContentRootPath, "ParkingImages");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/ParkingImages/{fileName}";

                return Ok(new
                {
                    success = true,
                    message = "File uploaded successfully",
                    fileName,
                    filePath = relativePath,
                    fullUrl = $"{Request.Scheme}://{Request.Host}{relativePath}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error uploading file: {ex.Message}" });
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("Delete")]
        public IActionResult Delete(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return BadRequest(new { success = false, message = "File name is required" });

                var uploadPath = Path.Combine(_environment.ContentRootPath, "ParkingImages");
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

                var uploadPath = Path.Combine(_environment.ContentRootPath, "ParkingImages");
                var filePath = Path.Combine(uploadPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Image not found");
                }

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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("SaveParkingLocation")]
        public async Task<IActionResult> SaveParkingLocation([FromForm] Models.DTOParkingProvider providerDTO)
        {
            try
            {
                var parkingUserId = GetAuthenticatedParkingUserId();
                if (parkingUserId == null)
                {
                    return Unauthorized(new { success = false, message = "SmartParking login required." });
                }

                providerDTO.UserId = parkingUserId.Value;

                var images = Request.Form.Files.GetFiles("images");
                if (images != null && images.Count > 0)
                {
                    var uploadFolder = Path.Combine(_environment.ContentRootPath, "ParkingImages");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    var imgSlots = new string?[] { providerDTO.img1, providerDTO.img2, providerDTO.img3, providerDTO.img4 };
                    var slotIndex = Array.FindIndex(imgSlots, string.IsNullOrEmpty);

                    foreach (var image in images)
                    {
                        if (slotIndex >= 4) break;
                        if (image == null || image.Length == 0) continue;

                        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                        var fileName = $"{Guid.NewGuid():N}{extension}";
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var relativePath = $"/ParkingImages/{fileName}";

                        if (slotIndex == 0) providerDTO.img1 = relativePath;
                        else if (slotIndex == 1) providerDTO.img2 = relativePath;
                        else if (slotIndex == 2) providerDTO.img3 = relativePath;
                        else if (slotIndex == 3) providerDTO.img4 = relativePath;

                        slotIndex++;
                        while (slotIndex < 4 && !string.IsNullOrEmpty(imgSlots[slotIndex])) slotIndex++;
                    }
                }

                var result = await _iParkingProvider.SaveParkingLocation(providerDTO, parkingUserId.Value);
                if (result.Contains("successfully"))
                {
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    providerDTO.img1 = ConvertToFullUrl(providerDTO.img1, baseUrl);
                    providerDTO.img2 = ConvertToFullUrl(providerDTO.img2, baseUrl);
                    providerDTO.img3 = ConvertToFullUrl(providerDTO.img3, baseUrl);
                    providerDTO.img4 = ConvertToFullUrl(providerDTO.img4, baseUrl);
                    return Ok(new { success = true, message = result, data = providerDTO });
                }

                return BadRequest(new { success = false, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetParkingLocations")]
        public async Task<IActionResult> GetParkingLocations()
        {
            try
            {
                var parkingUserId = GetAuthenticatedParkingUserId();
                if (parkingUserId == null)
                {
                    return Unauthorized(new { success = false, message = "SmartParking login required." });
                }

                var result = await _iParkingProvider.GetParkingLocationsByUser(parkingUserId.Value);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                foreach (var spot in result)
                {
                    spot.img1 = ConvertToFullUrl(spot.img1, baseUrl);
                    spot.img2 = ConvertToFullUrl(spot.img2, baseUrl);
                    spot.img3 = ConvertToFullUrl(spot.img3, baseUrl);
                    spot.img4 = ConvertToFullUrl(spot.img4, baseUrl);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("GetAllParkingLocations")]
        public async Task<IActionResult> GetAllParkingLocations()
        {
            try
            {
                var result = await _iParkingProvider.GetAllParkingLocations();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                foreach (var spot in result)
                {
                    spot.img1 = ConvertToFullUrl(spot.img1, baseUrl);
                    spot.img2 = ConvertToFullUrl(spot.img2, baseUrl);
                    spot.img3 = ConvertToFullUrl(spot.img3, baseUrl);
                    spot.img4 = ConvertToFullUrl(spot.img4, baseUrl);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("DeleteParkingLocation")]
        public async Task<IActionResult> DeleteParkingLocation(int uniqueId)
        {
            try
            {
                var parkingUserId = GetAuthenticatedParkingUserId();
                if (parkingUserId == null)
                {
                    return Unauthorized(new { success = false, message = "SmartParking login required." });
                }

                var result = await _iParkingProvider.DeleteParkingLocation(uniqueId, parkingUserId.Value);
                if (result.Contains("successfully"))
                {
                    return Ok(new { success = true, message = result });
                }

                return BadRequest(new { success = false, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        private string ConvertToFullUrl(string? imagePath, string baseUrl)
        {
            if (string.IsNullOrEmpty(imagePath))
                return imagePath ?? "";

            if (imagePath.StartsWith("http") || imagePath.StartsWith("data:"))
                return imagePath;

            return $"{baseUrl}{(imagePath.StartsWith("/") ? "" : "/")}{imagePath}";
        }

        private int? GetAuthenticatedParkingUserId()
        {
            var module = User.FindFirst("module")?.Value;
            if (!string.Equals(module, "SmartParking", StringComparison.Ordinal))
            {
                return null;
            }

            var claimValue =
                User.FindFirst("parking_user_id")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(claimValue, out var userId) ? userId : null;
        }
    }
}
