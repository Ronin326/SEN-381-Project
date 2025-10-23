using CampusLearn_Web_App.Models;

namespace CampusLearn_Web_App.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message);
        Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllUserNotificationsAsReadAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
    }
}