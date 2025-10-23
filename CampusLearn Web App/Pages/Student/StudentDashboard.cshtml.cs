using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using CampusLearn_Web_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLearn_Web_App.Pages.Student
{
	public class DashboardModel : PageModel
	{
		private readonly ChatbotService _chatbot;
		private readonly ContentFilterService _filter;
		private readonly CampusLearnDbContext _context;
		private readonly IUserService _userService;

		public DashboardModel(
			ChatbotService chatbot,
			ContentFilterService filter,
			CampusLearnDbContext context,
			IUserService userService)
		{
			_chatbot = chatbot;
			_filter = filter;
			_context = context;
			_userService = userService;
		}

		[BindProperty]
		public string UserMessage { get; set; }

		[BindProperty]
		public bool IsChatOpen { get; set; } = false;

		public List<(string Content, bool IsUser)> Messages { get; set; } = new();
		public List<StudentModule> StudentModules { get; set; } = new();

		public async Task<IActionResult> OnPostAsync()
		{
			if (!string.IsNullOrWhiteSpace(UserMessage))
			{
				if (!_filter.IsMessageAllowed(UserMessage, out var reason))
				{
					Messages.Add(("?? " + reason, false));
					return Page();
				}

				Messages.Add((UserMessage, true));

				var reply = await _chatbot.GetChatResponseAsync(UserMessage);
				Messages.Add((reply, false));
			}

			return Page();
		}

		public async Task OnGetAsync()
		{
			var userId = await GetCurrentUserIdAsync();
			Console.WriteLine("Current User Id: " + userId);

			if (userId == null)
			{
				// Not logged in, redirect to login page
				RedirectToPage("/LoginPage");
				return;
			}

			await LoadStudentModulesAsync(userId.Value);
		}

		// Helper method to reload student modules
		private async Task LoadStudentModulesAsync(int userId)
		{
			StudentModules = await _context.StudentModules
				.Include(sm => sm.Module)
				.Where(sm => sm.UserID == userId)
				.ToListAsync();
		}

		// ? Updated: Use session to get current logged-in user
		private async Task<int?> GetCurrentUserIdAsync()
		{
			var email = HttpContext.Session.GetString("UserEmail");
			Console.WriteLine("Current User Email: " + email);
			if (string.IsNullOrEmpty(email))
				return null;

			var user = await _userService.GetUserByEmailAsync(email);
			return user?.UserID;
		}
	}
}
