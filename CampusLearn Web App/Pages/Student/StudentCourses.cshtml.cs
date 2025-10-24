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
	public class MyCoursesModel : PageModel
	{
		private readonly ChatbotService _chatbot;
		private readonly ContentFilterService _filter;
		private readonly CampusLearnDbContext _context;
		private readonly IUserService _userService;

		public MyCoursesModel(
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
		public List<Module> Modules { get; set; } = new();
		public List<Module> SubscribedModules { get; set; } = new();

		// Helper to reload subscribed modules
		private async Task LoadSubscribedModulesAsync(int userId)
		{
			SubscribedModules = await _context.StudentModules
				.Where(sm => sm.UserID == userId)
				.Include(sm => sm.Module)
				.Select(sm => sm.Module)
				.ToListAsync();
		}

		private async Task<int?> GetCurrentUserIdAsync()
		{
			var email = HttpContext.Session.GetString("UserEmail");
			Console.WriteLine("Current User Email: " + email);
			if (string.IsNullOrEmpty(email))
				return null;

			var user = await _userService.GetUserByEmailAsync(email);
			return user?.UserID;
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

			// Load all modules for modal
			Modules = await _context.Modules.ToListAsync();

			// Load subscribed modules
			await LoadSubscribedModulesAsync(userId.Value);
		}

		public async Task<IActionResult> OnPostSubscribeAsync(int moduleId)
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null)
				return RedirectToPage("/LoginPage");

			bool alreadySubscribed = await _context.StudentModules
				.AnyAsync(sm => sm.UserID == userId.Value && sm.ModuleID == moduleId);

			if (alreadySubscribed)
			{
				TempData["Message"] = "You are already subscribed to this module.";
				return RedirectToPage();
			}

			var studentModule = new StudentModule
			{
				UserID = userId.Value,
				ModuleID = moduleId
			};

			_context.StudentModules.Add(studentModule);
			await _context.SaveChangesAsync();

			TempData["Message"] = "Successfully subscribed to module!";
			await LoadSubscribedModulesAsync(userId.Value);
			return Page();
		}

		public async Task<IActionResult> OnPostUnsubscribeAsync(int moduleId)
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null)
				return RedirectToPage("/LoginPage");

			var subscription = await _context.StudentModules
				.FirstOrDefaultAsync(sm => sm.UserID == userId.Value && sm.ModuleID == moduleId);

			if (subscription != null)
			{
				_context.StudentModules.Remove(subscription);
				await _context.SaveChangesAsync();
				TempData["Message"] = "Successfully unsubscribed from module!";
			}
			else
			{
				TempData["Message"] = "Subscription not found.";
			}

			await LoadSubscribedModulesAsync(userId.Value);
			return Page();
		}
	}
}
