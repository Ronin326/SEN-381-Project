using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CampusLearn_Web_App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampusLearn_Web_App.Pages
{
    public class TestChatModel : PageModel
    {
        private readonly ChatbotService _chatbot;
        private readonly ContentFilterService _filter;

        public TestChatModel(ChatbotService chatbot, ContentFilterService filter)
        {
            _chatbot = chatbot;
            _filter = filter;
        }

        [BindProperty]
        public string UserMessage { get; set; }

        public List<(string Content, bool IsUser)> Messages { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!_filter.IsMessageAllowed(UserMessage, out var reason))
            {
                Messages.Add(("⚠️ " + reason, false));
                return Page();
            }

            Messages.Add((UserMessage, true));

            var reply = await _chatbot.GetChatResponseAsync(UserMessage);
            Messages.Add((reply, false));

            return Page();
        }
    }
}
