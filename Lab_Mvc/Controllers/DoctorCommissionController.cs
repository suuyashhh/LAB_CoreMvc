using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorCommissionController : Controller
    {
        private readonly IDoctorCommission _docComRepository;

        public DoctorCommissionController(IDoctorCommission docComrepository)
        {
            this._docComRepository = docComrepository;
        }

        [HttpGet("DoctorCommissions")]
        public async Task<ActionResult> DoctorCommission([FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _docComRepository.GetDoctorCommission(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("DoctorCommission/{docCom_id}")]
        public async Task<ActionResult> GetDoctorCommissionById(long docCom_id, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _docComRepository.GetDoctorCommissionById(docCom_id, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetDateWiseDocCommission/{from_date},{to_date}")]
        public async Task<ActionResult> GetDateWiseDocCommission(string from_date, string to_date, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _docComRepository.GetDateWiseDocCommission(from_date, to_date, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveDoctorCommission")]
        public async Task<ActionResult<DTODoctorCommission>> SaveDoctorCommission(DTODoctorCommission objDocCom)
        {
            try
            {
                if (objDocCom == null)
                {
                    return BadRequest();
                }
                var createdProperty = _docComRepository.SaveDoctorCommission(objDocCom);
                var result = new
                {
                    data = objDocCom,
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
        [Route("EditDoctorCommission/{docCom_id}")]
        public async Task<ActionResult<DTODoctorCommission>> EditDoctorCommission(DTODoctorCommission objDocCom, long docCom_id)
        {
            try
            {
                if (objDocCom == null)
                {
                    return BadRequest();
                }
                var createdProperty = _docComRepository.EditDoctorCommission(objDocCom, docCom_id);
                var result = new
                {
                    data = objDocCom,
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
        [Route("DeleteDoctorCommission/{docCom_id}")]
        public async Task<ActionResult<DTODoctorCommission>> DeleteDoctorCommission(long docCom_id, [FromQuery] int comId)
        {
            try
            {
                if (docCom_id == null)
                {
                    return BadRequest();
                }
                var createdProperty = _docComRepository.DeleteDoctorCommission(docCom_id, comId);
                return Ok(docCom_id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }
    }
}
