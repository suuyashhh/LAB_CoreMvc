using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class DairyMastersController : ControllerBase
    {
        private readonly IDairyMasters _dairyMasters;

        public DairyMastersController(IDairyMasters dairyMasters)
        {
            _dairyMasters = dairyMasters;
        }

        // --- Animals ---
        [HttpGet("Animals/{userId:int}")]
        public async Task<IActionResult> GetAnimals(int userId)
        {
            // TODO: Prefer: get userId from User claims (authenticated user)
            var animals = await _dairyMasters.GetAnimalsByUser(userId);
            return Ok(animals);
        }

        [HttpPost("Animal")]
        public async Task<IActionResult> CreateAnimal([FromBody] AnimalDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.AnimalName)) return BadRequest("Invalid data");
            var id = await _dairyMasters.CreateAnimal(dto);
            return Ok(new { animalId = id });
        }

        [HttpPut("Animal/{id:int}")]
        public async Task<IActionResult> UpdateAnimal(int id, [FromBody] AnimalDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.AnimalName)) return BadRequest("Invalid data");
            dto.AnimalId = id;
            var ok = await _dairyMasters.UpdateAnimal(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("Animal/{id:int}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var ok = await _dairyMasters.DeleteAnimal(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // --- Feeds ---
        [HttpGet("Feeds/{userId:int}")]
        public async Task<IActionResult> GetFeeds(int userId)
        {
            var feeds = await _dairyMasters.GetFeedsByUser(userId);
            return Ok(feeds);
        }

        [HttpPost("Feed")]
        public async Task<IActionResult> CreateFeed([FromBody] FeedDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FeedName)) return BadRequest("Invalid data");
            var id = await _dairyMasters.CreateFeed(dto);
            return Ok(new { feedId = id });
        }

        [HttpPut("Feed/{id:int}")]
        public async Task<IActionResult> UpdateFeed(int id, [FromBody] FeedDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FeedName)) return BadRequest("Invalid data");
            dto.FeedId = id;
            var ok = await _dairyMasters.UpdateFeed(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("Feed/{id:int}")]
        public async Task<IActionResult> DeleteFeed(int id)
        {
            var ok = await _dairyMasters.DeleteFeed(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
