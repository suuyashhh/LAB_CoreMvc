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
    [Route("api/vegetable")] // Fallback explicit route mapping
    public class MarketVegetableController : ControllerBase
    {
        private readonly IVegetableRepository _vegetableRepository;

        public MarketVegetableController(IVegetableRepository vegetableRepository)
        {
            _vegetableRepository = vegetableRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vegetables = await _vegetableRepository.GetAllAsync();
            return Ok(vegetables);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vegetable = await _vegetableRepository.GetByIdAsync(id);
            if (vegetable == null) return NotFound(new { Message = "Vegetable not found." });
            return Ok(vegetable);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vegetable vegetable)
        {
            if (string.IsNullOrEmpty(vegetable.EngVegetableName))
            {
                return BadRequest(new { Message = "English Vegetable Name is required." });
            }

            int id = await _vegetableRepository.AddAsync(vegetable);
            vegetable.Id = id;
            return CreatedAtAction(nameof(GetById), new { id = vegetable.Id }, vegetable);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Vegetable vegetable)
        {
            if (id != vegetable.Id) return BadRequest(new { Message = "ID mismatch." });
            if (string.IsNullOrEmpty(vegetable.EngVegetableName))
            {
                return BadRequest(new { Message = "English Vegetable Name is required." });
            }

            bool updated = await _vegetableRepository.UpdateAsync(vegetable);
            if (!updated) return NotFound(new { Message = "Vegetable not found." });
            return Ok(new { Message = "Vegetable updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _vegetableRepository.DeleteAsync(id);
            if (!deleted) return NotFound(new { Message = "Vegetable not found." });
            return Ok(new { Message = "Vegetable deleted successfully." });
        }
    }
}
