using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElectricityBillController : Controller
    {
        private readonly IElectricityBill _electricityBillRepository;

        public ElectricityBillController(IElectricityBill electricitybillrepository)
        {
            this._electricityBillRepository = electricitybillrepository;
        }

        [HttpGet("ElectricityBills")]
        public async Task<ActionResult> ElectricityBill([FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _electricityBillRepository.GetElectricityBill(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("ElectricityBill/{elcBill_id}")]
        public async Task<ActionResult> GetElectricityBillById(long elcBill_id)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _electricityBillRepository.GetElectricityBillById(elcBill_id));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveElectricityBill")]
        public async Task<ActionResult<DTOElectricityBill>> SaveElectricityBill(DTOElectricityBill objElcBill)
        {
            try
            {
                if (objElcBill == null)
                {
                    return BadRequest();
                }
                var createdProperty = _electricityBillRepository.SaveElectricityBill(objElcBill);
                return Ok(this);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpPost]
        [Route("EditElectricityBill/{elcBill_id}")]
        public async Task<ActionResult<DTOElectricityBill>> EditElectricityBill(DTOElectricityBill objElcBill, long elcBill_id)
        {
            try
            {
                if (objElcBill == null)
                {
                    return BadRequest();
                }
                var createdProperty = _electricityBillRepository.EditElectricityBill(objElcBill, elcBill_id);
                return Ok(objElcBill);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpDelete]
        [Route("DeleteElectricityBill/{elcBill_id}")]
        public async Task<ActionResult<DTOElectricityBill>> DeleteElectricityBill(long elcBill_id)
        {
            try
            {
                if (elcBill_id == null)
                {
                    return BadRequest();
                }
                var createdProperty = _electricityBillRepository.DeleteElectricityBill(elcBill_id);
                return Ok(elcBill_id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }



    }
}
