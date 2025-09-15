using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusLearn_Web_App.Services
{
    public interface ITopicService
    {
        Task<Topic?> PostTopicAsync(int userId, int moduleId, string title, string? description);
        Task<Topic?> ReplyToTopicAsync(int topicId, int userId, string content);
        Task<IEnumerable<Topic>> GetModuleTopicsAsync(int moduleId);
        Task<Topic?> GetTopicByIdAsync(int topicId);
    }

    public class TopicService : ITopicService
    {
        private readonly CampusLearnDbContext _context;
        private readonly ILogger<TopicService> _logger;
        private readonly IUserService _userService;

        public TopicService(CampusLearnDbContext context, ILogger<TopicService> logger, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        public async Task<Topic?> PostTopicAsync(int userId, int moduleId, string title, string? description)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                var module = await _context.Modules.FindAsync(moduleId);

                if (user == null || module == null)
                {
                    _logger.LogWarning("❌ Failed to create topic - Invalid user ID {UserId} or module ID {ModuleId}", userId, moduleId);
                    return null;
                }

                var topic = new Topic
                {
                    Title = title,
                    Description = description,
                    CreationDate = DateTime.Now,
                    UserID = userId,
                    ModuleID = moduleId
                };

                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Topic created successfully: {Title} by {UserName} in module {ModuleId}", 
                    title, $"{user.FirstName} {user.LastName}", moduleId);

                return topic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error creating topic: {Error}", ex.Message);
                return null;
            }
        }

        public async Task<Topic?> ReplyToTopicAsync(int topicId, int userId, string content)
        {
            try
            {
                var topic = await _context.Topics.FindAsync(topicId);
                var user = await _userService.GetUserByIdAsync(userId);

                if (topic == null || user == null)
                {
                    _logger.LogWarning("❌ Failed to reply to topic - Invalid topic ID {TopicId} or user ID {UserId}", topicId, userId);
                    return null;
                }

                // Create a new topic that references the original topic
                var reply = new Topic
                {
                    Title = $"Re: {topic.Title}",
                    Description = content,
                    CreationDate = DateTime.Now,
                    UserID = userId,
                    ModuleID = topic.ModuleID
                };

                _context.Topics.Add(reply);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Reply posted successfully to topic {TopicId} by {UserName}", 
                    topicId, $"{user.FirstName} {user.LastName}");

                return reply;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error replying to topic: {Error}", ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Topic>> GetModuleTopicsAsync(int moduleId)
        {
            return await _context.Topics
                .Include(t => t.User)
                .Where(t => t.ModuleID == moduleId)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }

        public async Task<Topic?> GetTopicByIdAsync(int topicId)
        {
            return await _context.Topics
                .Include(t => t.User)
                .Include(t => t.Module)
                .FirstOrDefaultAsync(t => t.TopicID == topicId);
        }
    }
}