using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BikeFuleController : Controller
    {

        private readonly IBikeFule _bikeFuleRepository;

        public BikeFuleController(IBikeFule bikefulerepository)
        {
            this._bikeFuleRepository = bikefulerepository;
        }

        [HttpGet("BikeFules")]
        public async Task<ActionResult> BikeFule([FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _bikeFuleRepository.GetBikeFule(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("BikeFule/{bike_id}")]
        public async Task<ActionResult> GetBikeFuleById(long bike_id, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _bikeFuleRepository.GetBikeFuleById(bike_id, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetDateWiseBikeFule/{from_date},{to_date}")]
        public async Task<ActionResult> GetDateWiseBikeFule(string from_date, string to_date, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _bikeFuleRepository.GetDateWiseBikeFule(from_date, to_date, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveBikeFule")]
        public async Task<ActionResult<DTOBikeFule>> SaveBikeFule(DTOBikeFule objBike)
        {
            try
            {
                if (objBike == null)
                {
                    return BadRequest();
                }
                var createdProperty = _bikeFuleRepository.SaveBikeFule(objBike);
                var result = new
                {
                    data = objBike,
                    Message = "success"
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpPost]
        [Route("EditBikeFule/{bike_id}")]
        public async Task<ActionResult<DTOBikeFule>> EditBikeFule(DTOBikeFule objBike, long bike_id)
        {
            try
            {
                if (objBike == null)
                {
                    return BadRequest();
                }
                var createdProperty = _bikeFuleRepository.EditBikeFule(objBike, bike_id);
                var result = new
                {
                    data = objBike,
                    Message = "success"
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpDelete]
        [Route("DeleteBikeFule/{bike_id}")]
        public async Task<ActionResult<DTOBikeFule>> DeleteBikeFule(long bike_id, [FromQuery] int comId)
        {
            try
            {
                if (bike_id == null)
                {
                    return BadRequest();
                }
                var createdProperty = _bikeFuleRepository.DeleteBikeFule(bike_id, comId);
                return Ok(bike_id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }


    }
}
