using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Data;

namespace CampusLearn_Web_App.Pages.Student
{
	public class ChatsModel : PageModel
	{
		private readonly CampusLearnDbContext _context;
		private readonly ILogger<ChatsModel> _logger;

		public ChatsModel(CampusLearnDbContext context, ILogger<ChatsModel> logger)
		{
			_context = context;
			_logger = logger;
		}

		public void OnGet()
		{
			var userId = HttpContext.Session.GetString("UserId");
			var email = HttpContext.Session.GetString("UserEmail");

			_logger.LogInformation($"Chat page loaded by {email ?? "Unknown"} (UserId: {userId ?? "null"})");
		}

		// === AJAX Handler: return all users except the current one ===
		public async Task<IActionResult> OnGetUsersAsync()
		{
			try
			{
				var currentUserId = HttpContext.Session.GetString("UserId");

				if (string.IsNullOrEmpty(currentUserId))
					return new JsonResult(new { success = false, message = "User not logged in." });

				// Query all non-admin users, except the current one
				var users = await _context.Users
					.Where(u => u.Role != "Admin" && u.UserID.ToString() != currentUserId)
					.Select(u => new
					{
						userId = u.UserID,
						fullName = (u.FirstName + " " + u.LastName).Trim(),
						email = u.Email,
						role = u.Role
					})
					.OrderBy(u => u.fullName)
					.ToListAsync();

				return new JsonResult(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching chat users.");
				return new JsonResult(new { success = false, message = "Error loading users." });
			}
		}
	}
}
