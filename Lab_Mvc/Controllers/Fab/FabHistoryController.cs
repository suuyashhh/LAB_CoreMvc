using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabHistoryController : ControllerBase
    {
        private readonly IFabHistoryRepository _historyRepository;

        public FabHistoryController(IFabHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        [HttpGet("HistoryAll")]
        public async Task<IActionResult> GetAllHistory()
        {
            try
            {
                var history = await _historyRepository.GetAllHistory();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
