using Lab_Mvc.Interfaces;
using Lab_Mvc.Repositries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {

        private readonly ITest testRepository;

        //private readonly IMemoryCache _memoryCache;

        public TestController(ITest testrepository)
        {
            this.testRepository = testrepository;
            //this._memoryCache = memoryCache;
        }



        [HttpGet("Tests")]
        public async Task<ActionResult> Tests([FromQuery]  int comId)
        {
            try
            {
                return Ok(await testRepository.GetTests(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("Test/{test_code}")]
        public async Task<ActionResult> TestById(Int64 test_code)
        {
            var cacheKey = "MyKey";
            try
            {
                /*  var employeeList = await loginRepository.Getlogindetails();*/
                return Ok(await testRepository.GetTestById(test_code));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveTest")]
        public async Task<ActionResult<DTOTest>> SaveTest(DTOTest test)

        {
            try
            {
                if (test == null)
                {
                    return BadRequest();
                }
                var createdProperty = testRepository.SaveTest(test);
                var result = new
                {
                    data = test,
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
        [Route("EditTest/{test_code}")]
        public async Task<ActionResult<DTOTest>> EditTest(DTOTest test, long test_code)

        {
            try
            {
                if (test == null)
                {
                    return BadRequest();
                }
                var createdProperty = testRepository.EditTest(test, test_code);
                var result = new
                {
                    data = test,
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
        [Route("DeleteTest/{test_code}")]
        public async Task<ActionResult<DTOTest>> DeleteTest(long test_code)

        {
            try
            {
                if (test_code == null)
                {
                    return BadRequest();
                }
                var createdProperty = testRepository.DeleteTest(test_code);
                return Ok(test_code);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }
    }
}
