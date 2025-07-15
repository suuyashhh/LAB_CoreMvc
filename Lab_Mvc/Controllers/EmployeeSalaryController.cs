using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeSalaryController : Controller
    {

        private readonly IEmployeeSalary _empSalaryRepository;

        public EmployeeSalaryController(IEmployeeSalary empsalaryrepository)
        {
            this._empSalaryRepository = empsalaryrepository;
        }

        [HttpGet("EmployeeSalarys/{comId}")]
        public async Task<ActionResult> EmployeeSalary( int comId)
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
        public async Task<ActionResult> GetEmployeeSalaryById(long empSal_id)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await _empSalaryRepository.GetEmployeeSalaryById(empSal_id));
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
                return Ok(this);
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
                return Ok(objEmpSlry);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeSalary/{empSal_id}")]
        public async Task<ActionResult<DTOEmployeeSalary>> DeleteEmployeeSalary(long empSal_id)
        {
            try
            {
                if (empSal_id == null)
                {
                    return BadRequest();
                }
                var createdProperty = _empSalaryRepository.DeleteEmployeeSalary(empSal_id);
                return Ok(empSal_id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }




    }
}
