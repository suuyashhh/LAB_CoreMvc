using Lab_Mvc.Interfaces.Farm;
using Microsoft.AspNetCore.Mvc;
using Models.Farm;

namespace Lab_Mvc.Controllers.Farm
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeFarmController : ControllerBase
    {
        private readonly IHomeFarm _iHomeFarm;

        public HomeFarmController(IHomeFarm iHomeFarm)
        {
            _iHomeFarm = iHomeFarm;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string userId)
        {
            var data = await _iHomeFarm.GetAll(userId);
            return Ok(data);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] DTOHomeFarm model)
        {
            var result = await _iHomeFarm.Insert(model);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] DTOHomeFarm model)
        {
            var result = await _iHomeFarm.Update(model);
            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int farmId, string userId)
        {
            var result = await _iHomeFarm.Delete(farmId,userId);
            return Ok(result);
        }
    }
}
