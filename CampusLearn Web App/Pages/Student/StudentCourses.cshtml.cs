using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CampusLearn_Web_App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CampusLearn_Web_App.Pages.Student
{
    public class MyCoursesModel : PageModel
	{
		private readonly ChatbotService _chatbot;
		private readonly ContentFilterService _filter;

		public MyCoursesModel(ChatbotService chatbot, ContentFilterService filter)
		{
			_chatbot = chatbot;
			_filter = filter;
		}

		[BindProperty]
		public string UserMessage { get; set; }

		[BindProperty]
		public bool IsChatOpen { get; set; } = false; // bound to hidden input

		public List<(string Content, bool IsUser)> Messages { get; set; } = new();

		public async Task<IActionResult> OnPostAsync()
		{
			// IsChatOpen comes from the hidden input automatically

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

		public void OnGet()
		{
			// nothing extra needed here; chat state comes from default or postback
		}
	}
}
