using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeSalaryController : Controller
    {

        private readonly IEmployeeSalary _empSalaryRepository;

        public EmployeeSalaryController(IEmployeeSalary empsalaryrepository)
        {
            this._empSalaryRepository = empsalaryrepository;
        }

        [HttpGet("EmployeeSalarys")]
        public async Task<ActionResult> EmployeeSalary([FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _empSalaryRepository.GetEmployeeSalary(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("EmployeeSalary/{empSal_id}")]
        public async Task<ActionResult> GetEmployeeSalaryById(long empSal_id, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _empSalaryRepository.GetEmployeeSalaryById(empSal_id, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetDateWiseEmpSalary/{from_date},{to_date}")]
        public async Task<ActionResult> GetDateWiseEmpSalary(string from_date, string to_date, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _empSalaryRepository.GetDateWiseEmpSalary(from_date, to_date, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeSalary")]
        public async Task<ActionResult<DTOEmployeeSalary>> SaveEmployeeSalary(DTOEmployeeSalary objEmpSlry)
        {
            try
            {
                if (objEmpSlry == null)
                {
                    return BadRequest();
                }
                var createdProperty = _empSalaryRepository.SaveEmployeeSalary(objEmpSlry);
                var result = new
                {
                    data = objEmpSlry,
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
        [Route("EditEmployeeSalary/{empSal_id}")]
        public async Task<ActionResult<DTOEmployeeSalary>> EditEmployeeSalary(DTOEmployeeSalary objEmpSlry, long empSal_id)
        {
            try
            {
                if (objEmpSlry == null)
                {
                    return BadRequest();
                }
                var createdProperty = _empSalaryRepository.EditEmployeeSalary(objEmpSlry, empSal_id);
                var result = new
                {
                    data = objEmpSlry,
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
        [Route("DeleteEmployeeSalary/{empSal_id}")]
        public async Task<ActionResult<DTOEmployeeSalary>> DeleteEmployeeSalary(long empSal_id, [FromQuery] int comId)
        {
            try
            {
                if (empSal_id == null)
                {
                    return BadRequest();
                }
                var createdProperty = _empSalaryRepository.DeleteEmployeeSalary(empSal_id, comId);
                return Ok(empSal_id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }




    }
}
