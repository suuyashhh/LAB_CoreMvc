using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;
using System.Net;

namespace Lab_Mvc.Controllers.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorDairyController : Controller
    {
        private readonly IDoctorDairy _DoctorDairy;

        public DoctorDairyController(IDoctorDairy DoctorDairy)
        {
            _DoctorDairy = DoctorDairy;
        }

        [HttpGet("History/{userId}")]
        public async Task<IActionResult> GetAllDoctorHistory(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var feedHistory = await _DoctorDairy.GetAllDoctorHistory(userId);

                return Ok(feedHistory);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving feed history", error = ex.Message });
            }
        }

        [HttpGet("GetDocImageById/{exp_id}")]
        public async Task<IActionResult> GetDocImageById(long exp_id)
        {
            if (exp_id <= 0)
                return BadRequest(new { message = "Invalid expense ID" });

            var result = await _DoctorDairy.GetDocImageById(exp_id);

            if (result == null)
                return NotFound(new { message = "Feed image not found" });

            return Ok(result);
        }


        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] DTODoctorDairy objDDoc)
        {
            try
            {
                await _DoctorDairy.Save(objDDoc);

                return Ok(new { message = "Feed Saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while saving feed", error = ex.Message });
            }
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] DTODoctorDairy objDDoc)
        {
            try
            {
                if (objDDoc.expense_id <= 0)
                {
                    return BadRequest(new { message = "Invalid expense ID" });
                }

                if (objDDoc == null)
                {
                    return BadRequest(new { message = "Feed data cannot be null" });
                }

                await _DoctorDairy.Edit(objDDoc);

                return Ok(new { message = "Feed updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while updating feed", error = ex.Message });
            }
        }

        [HttpDelete("{exp_id}")]
        public async Task<IActionResult> Delete(long exp_id)
        {
            try
            {
                if (exp_id <= 0)
                {
                    return BadRequest(new { message = "Invalid expense ID" });
                }

                var existingFeed = await _DoctorDairy.GetDocImageById(exp_id);
                if (existingFeed == null)
                {
                    return NotFound(new { message = "Feed not found" });
                }

                await _DoctorDairy.Delete(exp_id);

                return Ok(new { message = "Feed deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while deleting feed", error = ex.Message });
            }
        }
    }
}
