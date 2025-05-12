using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly IEmployee employeeRepository;


        //private readonly IMemoryCache _memoryCache;

        public EmployeeController(IEmployee employeerepository)
        {
            this.employeeRepository = employeerepository;
            //this._memoryCache = memoryCache;
        }

        [HttpGet("Employees")]
        public async Task<ActionResult> Employees()
        {
            var cacheKey = "MyKey";
            try
            {
                /*  var employeeList = await loginRepository.Getlogindetails();*/
                return Ok(await employeeRepository.GetEmployees());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("Employee/{{emp_code}}")]
        public async Task<ActionResult> GetEmployeeById(long emp_code)
        {
            var cacheKey = "MyKey";
            try
            {
                /*  var employeeList = await loginRepository.Getlogindetails();*/
                return Ok(await employeeRepository.GetEmployeeById(emp_code));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveEmployee")]
        public async Task<ActionResult<DTOEmployee>> SaveEmployee(DTOEmployee emp)

        {
            try
            {
                if (emp == null)
                {
                    return BadRequest();
                }
                var createdProperty = employeeRepository.SaveEmployee(emp);
                return Ok(emp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpPost]
        [Route("EditEmployee/{{emp_code}}")]
        public async Task<ActionResult<DTOEmployee>> EditEmployee(DTOEmployee emp, long emp_code)

        {
            try
            {
                if (emp == null)
                {
                    return BadRequest();
                }
                var createdProperty = employeeRepository.EditEmployee(emp, emp_code);
                return Ok(emp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpDelete]
        [Route("DeleteEmployee/{{emp_code}}")]
        public async Task<ActionResult<DTOEmployee>> DeleteEmployee(long emp_code)

        {
            try
            {
                if (emp_code == null)
                {
                    return BadRequest();
                }
                var createdProperty = employeeRepository.DeleteEmployee(emp_code);
                return Ok(emp_code);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }
    }
}
