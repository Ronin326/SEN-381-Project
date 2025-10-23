using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;

namespace CampusLearn_Web_App.Pages.Chat
{
    public class ChatModel : PageModel
    {
        private readonly CampusLearnDbContext _context;

        public ChatModel(CampusLearnDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int ReceiverId { get; set; }

        [BindProperty]
        public string NewMessage { get; set; } = string.Empty;

        public int CurrentUserId { get; set; } = 1; // Replace with actual logged-in user id from session/auth
        public string ReceiverName { get; set; } = "User";
        public List<Message> Messages { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int receiverId)
        {
            ReceiverId = receiverId;

            var receiver = await _context.Users.FindAsync(receiverId);
            ReceiverName = receiver != null ? receiver.FirstName ?? receiver.Email : "Unknown User";

            Messages = await _context.Messages
                .Where(m =>
                    (m.SenderID == CurrentUserId && m.ReceiverID == ReceiverId) ||
                    (m.SenderID == ReceiverId && m.ReceiverID == CurrentUserId))
                .OrderBy(m => m.SentDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NewMessage))
                return RedirectToPage(new { receiverId = ReceiverId });

            var msg = new Message
            {
                SenderID = CurrentUserId,
                ReceiverID = ReceiverId,
                Content = NewMessage,
                SentDate = DateTime.UtcNow
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { receiverId = ReceiverId });
        }
    }
}
