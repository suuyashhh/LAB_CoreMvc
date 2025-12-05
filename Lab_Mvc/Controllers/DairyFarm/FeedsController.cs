using Lab_Mvc.Interfaces.DairyFarm;
using Microsoft.AspNetCore.Mvc;
using Models.DairyFarm;
using System.Net;

namespace Lab_Mvc.Controllers.DairyFarm
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedsController : ControllerBase
    {
        private readonly IFeeds _feeds;

        public FeedsController(IFeeds feeds)
        {
            _feeds = feeds;
        }

        [HttpGet("History/{userId}")]
        public async Task<IActionResult> GetAllFeedHistory(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var feedHistory = await _feeds.GetAllFeedHistory(userId);

                return Ok(feedHistory);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving feed history", error = ex.Message });
            }
        }

        [HttpGet("History/Detail/{exp_id}")]
        public async Task<IActionResult> GetFeedHistoryById(long exp_id)
        {
            try
            {
                if (exp_id <= 0)
                {
                    return BadRequest(new { message = "Invalid expense ID" });
                }

                var feed = await _feeds.GetFeedHistoryById(exp_id);

                if (feed == null)
                {
                    return NotFound(new { message = "Feed history not found" });
                }

                return Ok(feed);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while retrieving feed history details", error = ex.Message });
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] DTOFeeds objFeed)
        {
            try
            {
                if (objFeed == null)
                {
                    return BadRequest(new { message = "Feed data cannot be null" });
                }

                if (objFeed.user_id <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                if (string.IsNullOrWhiteSpace(objFeed.feed_name))
                {
                    return BadRequest(new { message = "Feed name is required" });
                }

                if (objFeed.price <= 0)
                {
                    return BadRequest(new { message = "Price must be greater than zero" });
                }

                if (objFeed.quantity <= 0)
                {
                    return BadRequest(new { message = "Quantity must be greater than zero" });
                }

                await _feeds.Save(objFeed);

                return CreatedAtAction(nameof(GetFeedHistoryById), new { exp_id = objFeed.expense_id },
                    new { message = "Feed saved successfully", data = objFeed });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "An error occurred while saving feed", error = ex.Message });
            }
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] DTOFeeds objFeed)
        {
            try
            {
                if (objFeed.expense_id <= 0)
                {
                    return BadRequest(new { message = "Invalid expense ID" });
                }

                if (objFeed == null)
                {
                    return BadRequest(new { message = "Feed data cannot be null" });
                }
                              
                await _feeds.Edit(objFeed);

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

                var existingFeed = await _feeds.GetFeedHistoryById(exp_id);
                if (existingFeed == null)
                {
                    return NotFound(new { message = "Feed not found" });
                }

                await _feeds.Delete(exp_id);

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