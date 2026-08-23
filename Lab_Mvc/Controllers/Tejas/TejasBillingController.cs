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

        [HttpGet]
        public async Task<IActionResult> GetAllBills()
        {
            var result = await _tejasBilling.GetAllBills();
            return Ok(result);
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
            if (bill.CreatedAt == default) bill.CreatedAt = System.DateTime.UtcNow;
            if (bill.UpdatedAt == default) bill.UpdatedAt = System.DateTime.UtcNow;

            bill.BillNumber = await _tejasBilling.GetNextBillNumber();

            await _tejasBilling.InsertBill(bill);
            return Ok(bill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(string id, [FromBody] TejasBill bill)
        {
            bill.Id = id;
            bill.UpdatedAt = System.DateTime.UtcNow;
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
