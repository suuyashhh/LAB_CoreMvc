using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;
using System.Net;

namespace Lab_Mvc.Repositries.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillDairyController : Controller
    {
        private readonly IBillDairy _BillDairy;

        public BillDairyController(IBillDairy BillDairy)
        {
            _BillDairy = BillDairy;
        }

        // ================= BILL HISTORY =================

        [HttpGet("History/{userId}")]
        public async Task<IActionResult> GetAllBillHistory(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var billHistory = await _BillDairy.GetAllBillHistory(userId);

                return Ok(billHistory);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving bill history", error = ex.Message });
            }
        }

        // ================= BILL IMAGE =================

        [HttpGet("GetBillImageById/{exp_id}")]
        public async Task<IActionResult> GetBillImageById(long exp_id)
        {
            if (exp_id <= 0)
                return BadRequest(new { message = "Invalid bill ID" });

            var result = await _BillDairy.GetBillImageById(exp_id);

            if (result == null)
                return NotFound(new { message = "Bill image not found" });

            return Ok(result);
        }

        // ================= SAVE BILL =================

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] DTOBillDairy objBill)
        {
            try
            {
                await _BillDairy.Save(objBill);

                return Ok(new { message = "Bill saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while saving bill", error = ex.Message });
            }
        }

        // ================= EDIT BILL =================

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] DTOBillDairy objBill)
        {
            try
            {
                if (objBill == null)
                {
                    return BadRequest(new { message = "Bill data cannot be null" });
                }

                if (objBill.bill_id <= 0)
                {
                    return BadRequest(new { message = "Invalid bill ID" });
                }

                await _BillDairy.Edit(objBill);

                return Ok(new { message = "Bill updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while updating bill", error = ex.Message });
            }
        }

        // ================= DELETE BILL =================

        [HttpDelete("{exp_id}")]
        public async Task<IActionResult> Delete(long exp_id)
        {
            try
            {
                if (exp_id <= 0)
                {
                    return BadRequest(new { message = "Invalid bill ID" });
                }

                var existingBill = await _BillDairy.GetBillImageById(exp_id);
                if (existingBill == null)
                {
                    return NotFound(new { message = "Bill not found" });
                }

                await _BillDairy.Delete(exp_id);

                return Ok(new { message = "Bill deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while deleting bill", error = ex.Message });
            }
        }
    }
}
