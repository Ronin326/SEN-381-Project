using CampusLearn_Web_App.Models;
using CampusLearn_Web_App.Services;
using Microsoft.AspNetCore.Mvc;
using CampusLearn_Web_App.Extensions;

namespace CampusLearn_Web_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly ILogger<TopicController> _logger;

        public TopicController(ITopicService topicService, ILogger<TopicController> logger)
        {
            _topicService = topicService;
            _logger = logger;
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostTopic([FromBody] PostTopicRequest request)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }

                var topic = await _topicService.PostTopicAsync(
                    currentUserId.Value,
                    request.ModuleID,
                    request.Title,
                    request.Description);

                if (topic != null)
                {
                    return Ok(new { success = true, message = "Topic posted successfully", data = topic });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to post topic" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting topic: {Error}", ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred while posting the topic" });
            }
        }

        [HttpPost("{topicId}/reply")]
        public async Task<IActionResult> ReplyToTopic(int topicId, [FromBody] ReplyTopicRequest request)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }

                var reply = await _topicService.ReplyToTopicAsync(
                    topicId,
                    currentUserId.Value,
                    request.Content);

                if (reply != null)
                {
                    return Ok(new { success = true, message = "Reply posted successfully", data = reply });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to post reply" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting reply: {Error}", ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred while posting the reply" });
            }
        }

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetModuleTopics(int moduleId)
        {
            try
            {
                var topics = await _topicService.GetModuleTopicsAsync(moduleId);
                return Ok(new { success = true, data = topics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving module topics: {Error}", ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving topics" });
            }
        }
    }

    public class PostTopicRequest
    {
        public int ModuleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class ReplyTopicRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}