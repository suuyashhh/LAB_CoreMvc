using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalHealthHistoryController : ControllerBase
    {
        private readonly IAnimalHealthHistory _animalHealthHistory;

        public AnimalHealthHistoryController(IAnimalHealthHistory animalHealthHistory)
        {
            _animalHealthHistory = animalHealthHistory;
        }

        // GET: api/AnimalHealthHistory/animals/{userId}
        [HttpGet("animals/{userId}")]
        public async Task<IActionResult> GetAllAnimals(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var animals = await _animalHealthHistory.GetAllAnimalsWithHealthSummary(userId);
                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving animals", error = ex.Message });
            }
        }

        // GET: api/AnimalHealthHistory/history/{userId}/{animalId}
        [HttpGet("history/{userId}/{animalId}")]
        public async Task<IActionResult> GetAnimalHistory(int userId, int animalId)
        {
            try
            {
                if (userId <= 0 || animalId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID or animal ID" });
                }

                var history = await _animalHealthHistory.GetAnimalHealthHistory(userId, animalId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving history", error = ex.Message });
            }
        }
    }
}