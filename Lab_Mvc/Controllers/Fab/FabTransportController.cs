using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabTransportController : ControllerBase
    {
        private readonly IFabTransportRepository _transportRepository;

        public FabTransportController(IFabTransportRepository transportRepository)
        {
            _transportRepository = transportRepository;
        }

        [HttpGet("Transport")]
        [HttpGet("Transport/History")]
        public async Task<IActionResult> GetTransport([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var transport = await _transportRepository.GetTransportHistory(from, to);
                return Ok(transport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
