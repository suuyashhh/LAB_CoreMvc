using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class BreedingDateCheckController : ControllerBase
    {
        private readonly IBreedingDateCheck _breedingDateCheck;

        public BreedingDateCheckController(IBreedingDateCheck breedingDateCheck)
        {
            _breedingDateCheck = breedingDateCheck;
        }

        // GET: api/BreedingDateCheck/animals/{userId}
        [HttpGet("animals/{userId}")]
        public async Task<IActionResult> GetAllAnimals(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var animals = await _breedingDateCheck.GetAllAnimalsWithBreedingSummary(userId);
                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving animals", error = ex.Message });
            }
        }

        // GET: api/BreedingDateCheck/history/{userId}/{animalId}
        [HttpGet("history/{userId}/{animalId}")]
        public async Task<IActionResult> GetAnimalBreedingHistory(int userId, int animalId)
        {
            try
            {
                if (userId <= 0 || animalId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID or animal ID" });
                }

                var history = await _breedingDateCheck.GetAnimalBreedingHistory(userId, animalId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving breeding history", error = ex.Message });
            }
        }
    }
}