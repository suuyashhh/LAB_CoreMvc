using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotification _iNotification;

        public NotificationController(INotification iNotification)
        {
            _iNotification = iNotification;
        }

        [HttpGet("Breeding/{userId}")]
        public async Task<IActionResult> GetBreeding(int userId)
        {
            var data = await _iNotification.GetBreedingNotifications(userId);
            return Ok(data);
        }

        [HttpGet("Count/{userId}")]
        public async Task<IActionResult> Count(int userId)
        {
            var count = await _iNotification.GetNotificationCount(userId);
            return Ok(count);
        }

        [HttpPost("Check/{id}")]
        public async Task<IActionResult> Check(int id)
        {
            await _iNotification.MarkAsChecked(id);
            return Ok();
        }
    }


}
