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
    public class LabMaterialsController : Controller
    {
        private readonly ILabMaterials labMaterialsRepository;

        public LabMaterialsController(ILabMaterials labmaterialsrepository)
        {
            this.labMaterialsRepository = labmaterialsrepository;
        }

        [HttpGet("LabMaterials")]
        public async Task<ActionResult> LabMaterials([FromQuery] int comId)
        {
            var cacheKey = "MyKey";
            try
            {
                return Ok(await labMaterialsRepository.GetLabMaterials(comId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("LabMaterial/{mat_id}")]
        public async Task<ActionResult> GetLabMaterialsById(long mat_id)
        {
            var cacheKey = "MyKey";
            try
            {
                /*  var employeeList = await loginRepository.Getlogindetails();*/
                return Ok(await labMaterialsRepository.GetLabMaterialsById(mat_id));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("SaveLabMaterials")]
        public async Task<ActionResult<DTOLabMaterials>> SaveLabMaterials(DTOLabMaterials objMat)
        {
            try
            {
                if (objMat == null)
                {
                    return BadRequest();
                }
                var createdProperty = labMaterialsRepository.SaveLabMaterials(objMat);
                var result = new
                {
                    data = objMat,
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
        [Route("EditLabMaterials/{mat_id}")]
        public async Task<ActionResult<DTOLabMaterials>> EditLabMaterials(DTOLabMaterials objMat, long mat_id)

        {
            try
            {
                if (objMat == null)
                {
                    return BadRequest();
                }
                var createdProperty = labMaterialsRepository.EditLabMaterials(objMat, mat_id);
                var result = new
                {
                    data = objMat,
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
        [Route("DeleteLabMaterials/{mat_id}")]
        public async Task<ActionResult<DTOLabMaterials>> DeleteLabMaterials(long mat_id)

        {
            try
            {
                if (mat_id == null)
                {
                    return BadRequest();
                }
                var createdProperty = labMaterialsRepository.DeleteLabMaterials(mat_id);
                return Ok(mat_id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Saving Data");
            }
        }
    }
}
