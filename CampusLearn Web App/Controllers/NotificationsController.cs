using Microsoft.AspNetCore.Mvc;
using CampusLearn_Web_App.Services;
using CampusLearn_Web_App.Extensions;

namespace CampusLearn_Web_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var count = await _notificationService.GetUnreadNotificationCountAsync(userId.Value);
            return Ok(new { count });
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized();

            await _notificationService.MarkNotificationAsReadAsync(id);
            return Ok();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized();

            await _notificationService.MarkAllUserNotificationsAsReadAsync(userId.Value);
            return Ok();
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestNotification()
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();

            await _notificationService.CreateNotificationAsync(userId.Value, "🔔 Test notification - This is working!");
            return Ok(new { message = "Test notification sent!" });
        }
    }
}