using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusLearn_Web_App.Services
{
    public interface IMessageService
    {
        Task<Message?> SendMessageAsync(int senderId, int receiverId, string content);
        Task<IEnumerable<Message>> GetUserMessagesAsync(int userId);
    }

    public class MessageService : IMessageService
    {
        private readonly CampusLearnDbContext _context;
        private readonly ILogger<MessageService> _logger;
        private readonly IUserService _userService;

        public MessageService(CampusLearnDbContext context, ILogger<MessageService> logger, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        public async Task<Message?> SendMessageAsync(int senderId, int receiverId, string content)
        {
            try
            {
                var sender = await _userService.GetUserByIdAsync(senderId);
                var receiver = await _userService.GetUserByIdAsync(receiverId);

                if (sender == null || receiver == null)
                {
                    _logger.LogWarning("❌ Message send failed - Invalid sender or receiver ID: Sender {SenderId}, Receiver {ReceiverId}", senderId, receiverId);
                    return null;
                }

                var message = new Message
                {
                    SenderID = senderId,
                    ReceiverID = receiverId,
                    Content = content,
                    SentDate = DateTime.Now
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Message sent successfully from {SenderName} to {ReceiverName}", 
                    $"{sender.FirstName} {sender.LastName}", 
                    $"{receiver.FirstName} {receiver.LastName}");

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error sending message from {SenderId} to {ReceiverId}: {Error}", 
                    senderId, receiverId, ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Message>> GetUserMessagesAsync(int userId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderID == userId || m.ReceiverID == userId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
        }
    }
}
