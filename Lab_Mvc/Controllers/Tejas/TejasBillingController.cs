using Lab_Mvc.Interfaces.Tejas;
using Microsoft.AspNetCore.Mvc;
using Models.Tejas;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Tejas
{
    [Route("api/[controller]")]
    [ApiController]
    public class TejasBillingController : ControllerBase
    {
        private readonly ITejasBilling _tejasBilling;

        public TejasBillingController(ITejasBilling tejasBilling)
        {
            _tejasBilling = tejasBilling;
        }

        private System.DateTime GetIndianStandardTime()
        {
            System.DateTime istTime;
            try
            {
                var tz = System.TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                istTime = System.TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.UtcNow, tz);
            }
            catch
            {
                istTime = System.DateTime.UtcNow.AddHours(5).AddMinutes(30);
            }
            return System.DateTime.SpecifyKind(istTime, System.DateTimeKind.Unspecified);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBills([FromQuery] System.DateTime? startDate, [FromQuery] System.DateTime? endDate, [FromQuery] long? shopId = null)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                var result = await _tejasBilling.GetBillsByDateRange(startDate.Value, endDate.Value, shopId);
                return Ok(result);
            }
            else
            {
                // Note: if no dates provided, you might want to limit to last 7 days to avoid loading everything, but preserving existing behavior for now.
                var result = await _tejasBilling.GetAllBills(shopId);
                return Ok(result);
            }
        }

        [HttpGet("NextBillNumber")]
        public async Task<IActionResult> GetNextBillNumber([FromQuery] long? shopId = null)
        {
            var nextNumber = await _tejasBilling.GetNextBillNumber(shopId);
            return Ok(new { billNumber = nextNumber });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillById(string id)
        {
            var result = await _tejasBilling.GetBillById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> InsertBill([FromBody] TejasBill bill)
        {
            if (string.IsNullOrEmpty(bill.Id))
            {
                bill.Id = "bill_" + System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            
            bill.CreatedAt = GetIndianStandardTime();
            bill.UpdatedAt = bill.CreatedAt;

            if (string.IsNullOrEmpty(bill.BillNumber))
            {
                bill.BillNumber = await _tejasBilling.GetNextBillNumber(bill.TEJAS_SHOPES_ID);
            }

            await _tejasBilling.InsertBill(bill);
            return Ok(bill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(string id, [FromBody] TejasBill bill)
        {
            bill.Id = id;
            bill.UpdatedAt = GetIndianStandardTime();
            var affected = await _tejasBilling.UpdateBill(bill);
            if (affected == 0) return NotFound();
            return Ok(bill);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(string id)
        {
            var affected = await _tejasBilling.DeleteBill(id);
            if (affected == 0) return NotFound();
            return Ok();
        }
    }
}
