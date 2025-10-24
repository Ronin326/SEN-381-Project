using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using CampusLearn_Web_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLearn_Web_App.Pages.Tutor
{
	public class TutorCoursesModel : PageModel
	{
		private readonly ChatbotService _chatbot;
		private readonly ContentFilterService _filter;
		private readonly CampusLearnDbContext _context;
		private readonly IUserService _userService;

		public TutorCoursesModel(
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

		[BindProperty] public string UserMessage { get; set; }
		[BindProperty] public bool IsChatOpen { get; set; } = false;

		public List<(string Content, bool IsUser)> Messages { get; set; } = new();
		public List<Module> Modules { get; set; } = new();
		public List<Module> SubscribedModules { get; set; } = new();
		public List<Module> SubscribedTutorModules { get; set; } = new();

		private async Task<int?> GetCurrentUserIdAsync()
		{
			var email = HttpContext.Session.GetString("UserEmail");
			if (string.IsNullOrEmpty(email)) return null;

			var user = await _userService.GetUserByEmailAsync(email);
			return user?.UserID;
		}

		private async Task LoadSubscribedModulesAsync(int userId)
		{
			// Student courses
			SubscribedModules = await _context.StudentModules
				.Where(sm => sm.UserID == userId)
				.Include(sm => sm.Module)
				.Select(sm => sm.Module)
				.ToListAsync();

			// Tutor courses
			SubscribedTutorModules = await _context.TutorModules
				.Where(tm => tm.UserID == userId)
				.Include(tm => tm.Module)
				.Select(tm => tm.Module)
				.ToListAsync();
		}

		public async Task OnGetAsync()
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null) { RedirectToPage("/LoginPage"); return; }

			Modules = await _context.Modules.ToListAsync();
			await LoadSubscribedModulesAsync(userId.Value);
		}

		// Student Module Handlers
		public async Task<IActionResult> OnPostSubscribeAsync(int moduleId)
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null) return RedirectToPage("/LoginPage");

			if (await _context.StudentModules.AnyAsync(sm => sm.UserID == userId && sm.ModuleID == moduleId))
			{
				TempData["Message"] = "Already subscribed.";
				return RedirectToPage();
			}

			_context.StudentModules.Add(new StudentModule { UserID = userId.Value, ModuleID = moduleId });
			await _context.SaveChangesAsync();
			await LoadSubscribedModulesAsync(userId.Value);
			return Page();
		}

		public async Task<IActionResult> OnPostUnsubscribeAsync(int moduleId)
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null) return RedirectToPage("/LoginPage");

			var subscription = await _context.StudentModules
				.FirstOrDefaultAsync(sm => sm.UserID == userId && sm.ModuleID == moduleId);
			if (subscription != null) _context.StudentModules.Remove(subscription);

			await _context.SaveChangesAsync();
			await LoadSubscribedModulesAsync(userId.Value);
			return Page();
		}

		// Tutor Module Handlers
		public async Task<IActionResult> OnPostAddAsync(int moduleId)
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null) return RedirectToPage("/LoginPage");

			if (await _context.TutorModules.AnyAsync(tm => tm.UserID == userId && tm.ModuleID == moduleId))
			{
				TempData["Message"] = "Already teaching this module.";
				return RedirectToPage();
			}

			_context.TutorModules.Add(new TutorModule { UserID = userId.Value, ModuleID = moduleId });
			await _context.SaveChangesAsync();
			await LoadSubscribedModulesAsync(userId.Value);
			return Page();
		}

		public async Task<IActionResult> OnPostRemoveAsync(int moduleId)
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null) return RedirectToPage("/LoginPage");

			var module = await _context.TutorModules
				.FirstOrDefaultAsync(tm => tm.UserID == userId && tm.ModuleID == moduleId);
			if (module != null) _context.TutorModules.Remove(module);

			await _context.SaveChangesAsync();
			await LoadSubscribedModulesAsync(userId.Value);
			return Page();
		}
	}
}
