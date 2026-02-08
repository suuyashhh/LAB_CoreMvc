using Lab_Mvc.Interfaces;
using Lab_Mvc.Repositries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Lab_Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CasePaperController : Controller
    {
        private readonly ICasePaper casePaperRepository;

        //private readonly IMemoryCache _memoryCache;

        public CasePaperController(ICasePaper casepaperrepository)
        {
            this.casePaperRepository = casepaperrepository;
            //this._memoryCache = memoryCache;
        }

        [HttpGet("CasePapers")]
        public async Task<ActionResult> CasePapers([FromQuery] int comId)
        {
            try
            {
                return Ok(await casePaperRepository.GetCasePapers(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("CasePaper/{trn_no}")]
        public async Task<ActionResult> CasePaperById(long trn_no, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                /*  var employeeList = await loginRepository.Getlogindetails();*/
                return Ok(await casePaperRepository.GetCasePaperById(trn_no, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetDateWiseCasePaper/{from_date},{to_date}")]
        public async Task<ActionResult> GetDateWiseCasePaper(string from_date, string to_date, [FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await casePaperRepository.GetDateWiseCasePaper(from_date, to_date, comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveCasePaper")]
        public async Task<ActionResult<DTOCasePaper>> SaveCasePaper(DTOCasePaper casepaper)

        {
            try
            {
                if (casepaper == null)
                {
                    return BadRequest();
                }
                var createdProperty = casePaperRepository.SaveCasePaper(casepaper);
                var result = new
                {
                    data = casepaper,
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
        [Route("EditCasePaper/{trn_no}")]
        public async Task<ActionResult<DTOCasePaper>> EditCasePaper([FromBody] DTOCasePaper casepaper, [FromRoute] Int64 trn_no)

        {
            try
            {
                if (casepaper == null)
                {
                    return BadRequest();
                }
                var createdProperty = casePaperRepository.EditCasePaper(casepaper, trn_no);
                var result = new
                {
                    data = casepaper,
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
        [Route("DeleteCasePaper/{trn_no}")]
        public async Task<ActionResult<DTOCasePaper>> DeleteCasePaper(long trn_no, [FromQuery] int comId)

        {
            try
            {
                if (trn_no == null)
                {
                    return BadRequest();
                }
                var createdProperty = casePaperRepository.DeleteCasePaper(trn_no, comId);
                return Ok(trn_no);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }


        [HttpPost("ApproveCasePapers")]
        public async Task<IActionResult> ApproveCasePapers([FromBody] ApproveRequest request)
        {
            if (request.TrnNumbers == null || !request.TrnNumbers.Any())
                return BadRequest("No transaction numbers provided.");

            await casePaperRepository.ApproveCasePapers(request.TrnNumbers);
            return Ok(new { message = "Case papers approved successfully" });
        }

        public class ApproveRequest
        {
            public List<long> TrnNumbers { get; set; }
        }



    }
}
