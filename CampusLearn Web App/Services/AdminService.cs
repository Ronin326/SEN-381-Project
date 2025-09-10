using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn_Web_App.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserRoleAsync(int userId, string newRole, int adminId);
        Task<bool> DeleteUserAsync(int userId, int adminId);
        Task<bool> CreateAdminUserAsync(string firstName, string lastName, string email, string password, int currentAdminId);
        Task<Dictionary<string, int>> GetUserStatisticsAsync();
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
        Task<bool> ResetUserPasswordAsync(int userId, string newPassword, int adminId);
    }

    public class AdminService : IAdminService
    {
        private readonly CampusLearnDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IUserService _userService;
        private readonly ILogger<AdminService> _logger;

        public AdminService(CampusLearnDbContext context, IPasswordService passwordService, IUserService userService, ILogger<AdminService> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole, int adminId)
        {
            // Verify admin permissions
            var admin = await _userService.GetUserByIdAsync(adminId);
            if (admin?.Role != "Admin")
                return false;

            // Validate role
            if (!new[] { "Student", "Tutor", "Admin" }.Contains(newRole))
                return false;

            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return false;

            user.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId, int adminId)
        {
            // Verify admin permissions
            var admin = await _userService.GetUserByIdAsync(adminId);
            if (admin?.Role != "Admin")
                return false;

            // Don't allow admin to delete themselves
            if (userId == adminId)
                return false;

            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateAdminUserAsync(string firstName, string lastName, string email, string password, int currentAdminId)
        {
            // Verify current user is admin
            var currentAdmin = await _userService.GetUserByIdAsync(currentAdminId);
            if (currentAdmin?.Role != "Admin")
                return false;

            // Check if email exists
            if (await _userService.EmailExistsAsync(email))
                return false;

            var newAdmin = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = _passwordService.HashPassword(password),
                Role = "Admin"
            };

            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, int>> GetUserStatisticsAsync()
        {
            return await _context.Users
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Role, x => x.Count);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllUsersAsync();

            searchTerm = searchTerm.ToLower();
            return await _context.Users
                .Where(u => u.FirstName.ToLower().Contains(searchTerm) ||
                           u.LastName.ToLower().Contains(searchTerm) ||
                           u.Email.ToLower().Contains(searchTerm) ||
                           u.Role.ToLower().Contains(searchTerm))
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<bool> ResetUserPasswordAsync(int userId, string newPassword, int adminId)
        {
            // Verify admin permissions
            var admin = await _userService.GetUserByIdAsync(adminId);
            if (admin?.Role != "Admin")
                return false;

            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return false;

            user.PasswordHash = _passwordService.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
