using System.Text.Json;

namespace CampusLearn_Web_App.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }

        // Helper methods for common session operations
        public static bool IsUserLoggedIn(this ISession session)
        {
            return session.GetInt32("UserId").HasValue;
        }

        public static int? GetCurrentUserId(this ISession session)
        {
            return session.GetInt32("UserId");
        }

        public static string? GetCurrentUserRole(this ISession session)
        {
            return session.GetString("UserRole");
        }

        public static string? GetCurrentUserName(this ISession session)
        {
            return session.GetString("UserName");
        }

        public static string? GetCurrentUserEmail(this ISession session)
        {
            return session.GetString("UserEmail");
        }

        public static bool IsInRole(this ISession session, string role)
        {
            var userRole = session.GetString("UserRole");
            return userRole?.Equals(role, StringComparison.OrdinalIgnoreCase) == true;
        }

        public static bool IsAdmin(this ISession session)
        {
            return session.IsInRole("Admin");
        }

        public static bool IsTutor(this ISession session)
        {
            return session.IsInRole("Tutor");
        }

        public static bool IsStudent(this ISession session)
        {
            return session.IsInRole("Student");
        }

        public static void ClearUserSession(this ISession session)
        {
            session.Remove("UserId");
            session.Remove("UserEmail");
            session.Remove("UserRole");
            session.Remove("UserName");
        }
    }
}
