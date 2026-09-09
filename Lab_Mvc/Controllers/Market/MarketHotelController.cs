using System.Threading.Tasks;
using Models.Market;
using Lab_Mvc.Interfaces.Market;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers.Market
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/hotel")] // Fallback explicit route mapping
    public class MarketHotelController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;

        public MarketHotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hotels = await _hotelRepository.GetAllAsync();
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null) return NotFound(new { Message = "Hotel not found." });
            return Ok(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Hotel hotel)
        {
            if (string.IsNullOrEmpty(hotel.HotelName))
            {
                return BadRequest(new { Message = "Hotel Name is required." });
            }

            int id = await _hotelRepository.AddAsync(hotel);
            hotel.Id = id;
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Hotel hotel)
        {
            if (id != hotel.Id) return BadRequest(new { Message = "ID mismatch." });
            if (string.IsNullOrEmpty(hotel.HotelName))
            {
                return BadRequest(new { Message = "Hotel Name is required." });
            }

            bool updated = await _hotelRepository.UpdateAsync(hotel);
            if (!updated) return NotFound(new { Message = "Hotel not found." });
            return Ok(new { Message = "Hotel updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _hotelRepository.DeleteAsync(id);
            if (!deleted) return NotFound(new { Message = "Hotel not found." });
            return Ok(new { Message = "Hotel deleted successfully." });
        }
    }
}
