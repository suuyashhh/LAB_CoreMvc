using Lab_Mvc.Interfaces;
using Lab_Mvc.Repositries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : Controller
    {

        private readonly IDoctor doctorRepository;


        //private readonly IMemoryCache _memoryCache;

        public DoctorController(IDoctor doctorrepository)
        {
            this.doctorRepository = doctorrepository;
            //this._memoryCache = memoryCache;
        }


        [HttpGet("Doctors/{comId}")]
        public async Task<ActionResult> GetDoctors( int comId)
        {
            try
            {
                return Ok(await doctorRepository.GetDoctors(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("Doctor/{doctor_code}")]
        public async Task<ActionResult> DoctorById(long doctor_code)
        {
            var cacheKey = "MyKey";
            try
            {
                /*  var employeeList = await loginRepository.Getlogindetails();*/
                return Ok(await doctorRepository.GetDoctorById(doctor_code));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveDoctor")]
        public async Task<ActionResult<DTODoctor>> SaveDoctor(DTODoctor doctor)

        {
            try
            {
                if (doctor == null)
                {
                    return BadRequest();
                }
                var createdProperty = doctorRepository.SaveDoctor(doctor);
                return Ok(doctor);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpPost]
        [Route("EditDoctor/{doctor_code}")]
        public async Task<ActionResult<DTODoctor>> EditDoctor(DTODoctor doctor, long doctor_code)

        {
            try
            {
                if (doctor == null)
                {
                    return BadRequest();
                }
                var createdProperty = doctorRepository.EditDoctor(doctor, doctor_code);
                return Ok(doctor);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

        [HttpDelete]
        [Route("DeleteDoctor/{doctor_code}")]
        public async Task<ActionResult<DTODoctor>> DeleteDoctor(long doctor_code)

        {
            try
            {
                if (doctor_code == null)
                {
                    return BadRequest();
                }
                var createdProperty = doctorRepository.DeleteDoctor(doctor_code);
                return Ok(doctor_code);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }

    }
}
