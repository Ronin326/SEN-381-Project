using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn_Web_App.Pages.Student
{
    public class ChatsModel : PageModel
    {
        private readonly CampusLearnDbContext _context;

        public ChatsModel(CampusLearnDbContext context)
        {
            _context = context;
        }

        public void OnGet() { }

        // GET /Student/Chats?handler=Members
        public async Task<IActionResult> OnGetMembersAsync()
        {
            var currentUserId = HttpContext.Session.GetInt32("userid");

            // Only include Students or Tutors
            var allowedRoles = new[] { "Student", "Tutor" };

            var users = await _context.Users
                .Where(u =>
                    (currentUserId == null || u.UserID != currentUserId.Value) &&
                    allowedRoles.Contains(u.Role))
                .Select(u => new
                {
                    userId = u.UserID,
                    fullName = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    role = u.Role
                })
                .OrderBy(u => u.fullName)
                .ToListAsync();

            return new JsonResult(users);
        }

        // GET /Student/Chats?handler=Threads
        public async Task<IActionResult> OnGetThreadsAsync()
        {
            var currentUserId = HttpContext.Session.GetInt32("userid");
            if (currentUserId is null) return Unauthorized();
            var me = currentUserId.Value;

            var allowedRoles = new[] { "student", "tutor" };

            var baseQuery = _context.Messages
                .Where(m => m.SenderID == me || m.ReceiverID == me)
                .Select(m => new
                {
                    m.MessageID,
                    m.Content,
                    m.SentDate,
                    m.SenderID,
                    m.ReceiverID,
                    OtherId = (m.SenderID == me) ? m.ReceiverID : m.SenderID
                });

            var threads = await baseQuery
                .GroupBy(x => x.OtherId)
                .Select(g => g.OrderByDescending(x => x.SentDate).First())
                .Join(_context.Users,
                      x => x.OtherId,
                      u => u.UserID,
                      (x, u) => new
                      {
                          userId = u.UserID,
                          fullName = u.FirstName + " " + u.LastName,
                          email = u.Email,
                          role = u.Role,
                          lastMessage = x.Content,
                          lastAt = x.SentDate
                      })
                .Where(t => allowedRoles.Contains(t.role.ToLower()))
                .OrderByDescending(t => t.lastAt)
                .Take(50)
                .ToListAsync();

            return new JsonResult(threads);
        }

        // GET /Student/Chats?handler=Messages&otherUserId=123
        public async Task<IActionResult> OnGetMessagesAsync(int otherUserId)
        {
            var currentUserId = HttpContext.Session.GetInt32("userid");
            if (currentUserId == null) return Unauthorized();

            var me = currentUserId.Value;

            var messages = await _context.Messages
                .Where(m => (m.SenderID == me && m.ReceiverID == otherUserId) ||
                            (m.SenderID == otherUserId && m.ReceiverID == me))
                .OrderBy(m => m.SentDate)
                .Select(m => new
                {
                    id = m.MessageID,
                    senderId = m.SenderID,
                    receiverId = m.ReceiverID,
                    content = m.Content,
                    sentDate = m.SentDate
                })
                .ToListAsync();

            return new JsonResult(new { currentUserId = me, messages });
        }

        // POST /Student/Chats?handler=Message
        public class CreateMessageDto
        {
            public int ReceiverId { get; set; }
            public string Content { get; set; } = string.Empty;
        }

        // For production, consider [ValidateAntiForgeryToken] and send the token header from JS
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostMessageAsync([FromBody] CreateMessageDto dto)
        {
            var currentUserId = HttpContext.Session.GetInt32("userid");
            if (currentUserId == null) return Unauthorized();

            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Message content is required.");

            var receiverExists = await _context.Users.AnyAsync(u => u.UserID == dto.ReceiverId);
            if (!receiverExists) return NotFound("Receiver not found.");

            var message = new Message
            {
                SenderID = currentUserId.Value,
                ReceiverID = dto.ReceiverId,
                Content = dto.Content,
                SentDate = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                id = message.MessageID,
                senderId = message.SenderID,
                receiverId = message.ReceiverID,
                content = message.Content,
                sentDate = message.SentDate
            });
        }
    }
}
