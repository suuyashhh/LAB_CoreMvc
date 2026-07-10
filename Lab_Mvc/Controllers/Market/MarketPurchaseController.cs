using System;
using System.IO;
using System.Threading.Tasks;
using Models.Market;
using Lab_Mvc.Interfaces.Market;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers.Market
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/purchase")] // Fallback explicit route mapping
    public class MarketPurchaseController : ControllerBase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public MarketPurchaseController(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var purchases = await _purchaseRepository.GetAllAsync();
            return Ok(purchases);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null) return NotFound(new { Message = "Purchase entry not found." });
            return Ok(purchase);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseEntry entry)
        {
            if (entry.HotelId <= 0) return BadRequest(new { Message = "Hotel is required." });
            if (entry.Items == null || entry.Items.Count == 0) return BadRequest(new { Message = "At least one vegetable item is required." });

            int newId = await _purchaseRepository.AddAsync(entry);
            entry.Id = newId;
            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseEntry entry)
        {
            if (id != entry.Id) return BadRequest(new { Message = "ID mismatch." });
            if (entry.HotelId <= 0) return BadRequest(new { Message = "Hotel is required." });
            if (entry.Items == null || entry.Items.Count == 0) return BadRequest(new { Message = "At least one vegetable item is required." });

            bool updated = await _purchaseRepository.UpdateAsync(entry);
            if (!updated) return NotFound(new { Message = "Purchase entry not found." });
            return Ok(new { Message = "Purchase updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _purchaseRepository.DeleteAsync(id);
            if (!deleted) return NotFound(new { Message = "Purchase entry not found." });
            return Ok(new { Message = "Purchase deleted successfully." });
        }

        [HttpPost("upload")]
        [AllowAnonymous] // Anyone authenticated or guest upload
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { Message = "No file uploaded." });
            
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            
            var fileUrl = $"/uploads/{uniqueFileName}";
            return Ok(new { Url = fileUrl });
        }
    }
}
