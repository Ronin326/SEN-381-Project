using CampusLearn_Web_App.Models;
using CampusLearn_Web_App.Services;
using Microsoft.AspNetCore.Mvc;
using CampusLearn_Web_App.Extensions;

namespace CampusLearn_Web_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService, ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }

                var message = await _messageService.SendMessageAsync(currentUserId.Value, request.ReceiverID, request.Content);
                
                if (message != null)
                {
                    return Ok(new { success = true, message = "Message sent successfully", data = message });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to send message" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message: {Error}", ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred while sending the message" });
            }
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetUserMessages()
        {
            try
            {
                var currentUserId = HttpContext.Session.GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }

                var messages = await _messageService.GetUserMessagesAsync(currentUserId.Value);
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages: {Error}", ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving messages" });
            }
        }
    }

    public class SendMessageRequest
    {
        public int ReceiverID { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
