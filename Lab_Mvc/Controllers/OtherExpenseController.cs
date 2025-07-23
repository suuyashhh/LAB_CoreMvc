using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OtherExpenseController : Controller
    {
        private readonly IOtherExpense _otherExpaenseRepository;

        public OtherExpenseController(IOtherExpense otherexpenserepository)
        {
            this._otherExpaenseRepository = otherexpenserepository;
        }

        [HttpGet("OtherExpenses")]
        public async Task<ActionResult> OtherExpense([FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _otherExpaenseRepository.GetOtherExpense(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("OtherExpense/{otherEx_id}")]
        public async Task<ActionResult> GetOtherExpenseById(long otherEx_id)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _otherExpaenseRepository.GetOtherExpenseById(otherEx_id));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetDateWiseOthMaterials/{from_date},{to_date}")]
        public async Task<ActionResult> GetDateWiseOthMaterials(string from_date, string to_date)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _otherExpaenseRepository.GetDateWiseOthMaterials(from_date, to_date));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveOtherExpense")]
        public async Task<ActionResult<DTOOtherExpense>> SaveOtherExpense(DTOOtherExpense objOtherEx)
        {
            try
            {
                if (objOtherEx == null)
                {
                    return BadRequest();
                }
                var createdProperty = _otherExpaenseRepository.SaveOtherExpense(objOtherEx);
                var result = new
                {
                    data = objOtherEx,
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
        [Route("EditOtherExpense/{otherEx_id}")]
        public async Task<ActionResult<DTOOtherExpense>> EditOtherExpense(DTOOtherExpense objOtherEx, long otherEx_id)
        {
            try
            {
                if (objOtherEx == null)
                {
                    return BadRequest();
                }
                var createdProperty = _otherExpaenseRepository.EditOtherExpense(objOtherEx, otherEx_id);
                var result = new
                {
                    data = objOtherEx,
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
        [Route("DeleteOtherExpense/{otherEx_id}")]
        public async Task<ActionResult<DTOOtherExpense>> DeleteOtherExpense(long otherEx_id)
        {
            try
            {
                if (otherEx_id == null)
                {
                    return BadRequest();
                }
                var createdProperty = _otherExpaenseRepository.DeleteOtherExpense(otherEx_id);
                return Ok(otherEx_id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }
    }
}
