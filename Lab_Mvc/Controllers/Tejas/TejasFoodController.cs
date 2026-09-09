using Lab_Mvc.Interfaces.Tejas;
using Microsoft.AspNetCore.Mvc;
using Models.Tejas;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Tejas
{
    [Route("api/[controller]")]
    [ApiController]
    public class TejasFoodController : ControllerBase
    {
        private readonly ITejasFood _tejasFood;

        public TejasFoodController(ITejasFood tejasFood)
        {
            _tejasFood = tejasFood;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] long? shopId = null)
        {
            var result = await _tejasFood.GetAll(shopId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _tejasFood.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] TejasFoodItem item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = "food_" + System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            await _tejasFood.Insert(item);
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TejasFoodItem item)
        {
            item.Id = id;
            var affected = await _tejasFood.Update(item);
            if (affected == 0) return NotFound();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var affected = await _tejasFood.Delete(id);
            if (affected == 0) return NotFound();
            return Ok();
        }
    }
}
