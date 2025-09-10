using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn_Web_App.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> RegisterUserAsync(string firstName, string lastName, string email, string password, string role);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ValidateUserAsync(string email, string password);
        Task<bool> CanCreateAdminAsync(int currentUserId);
        Task<bool> PromoteToAdminAsync(int targetUserId, int currentAdminId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId);
    }

    public class UserService : IUserService
    {
        private readonly CampusLearnDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserService> _logger;

        public UserService(CampusLearnDbContext context, IPasswordService passwordService, ILogger<UserService> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _logger = logger;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("🔍 Searching for user with email: {Email}", email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                _logger.LogInformation("✅ User found: {FirstName} {LastName} (Role: {Role})", user.FirstName, user.LastName, user.Role);
            }
            else
            {
                _logger.LogWarning("❌ No user found with email: {Email}", email);
            }
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string email, string password, string role)
        {
            _logger.LogInformation("🚀 Registration attempt - Name: {FirstName} {LastName}, Email: {Email}, Role: {Role}", firstName, lastName, email, role);

            // SECURITY: Prevent admin creation through registration
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("🚨 SECURITY ALERT: Attempted admin creation through registration blocked for email: {Email}", email);
                throw new UnauthorizedAccessException("Admin accounts cannot be created through registration.");
            }

            // Only allow Student or Tutor roles
            if (!role.Equals("Student", StringComparison.OrdinalIgnoreCase) && 
                !role.Equals("Tutor", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("❌ Invalid role attempted: {Role} for email: {Email}", role, email);
                throw new ArgumentException("Invalid role. Only 'Student' and 'Tutor' roles are allowed for registration.");
            }

            // DOMAIN RESTRICTION: Only allow Belgium Campus email addresses
            if (!email.EndsWith("@belgiumcampus.ac.za", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("🚨 DOMAIN RESTRICTION: Non-Belgium Campus email blocked: {Email}", email);
                throw new ArgumentException("Only Belgium Campus email addresses (@belgiumcampus.ac.za) are allowed for registration.");
            }

            if (await EmailExistsAsync(email))
            {
                _logger.LogWarning("❌ Registration failed - Email already exists: {Email}", email);
                return false;
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = _passwordService.HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("✅ User successfully registered: {FirstName} {LastName} ({Email}) as {Role}", firstName, lastName, email, role);
            return true;
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            _logger.LogInformation("🔐 Login attempt for email: {Email}", email);
            
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("❌ Login failed - User not found: {Email}", email);
                return false;
            }

            var isValidPassword = _passwordService.VerifyPassword(password, user.PasswordHash);
            if (isValidPassword)
            {
                _logger.LogInformation("✅ Login successful for: {FirstName} {LastName} ({Email}) - Role: {Role}", 
                    user.FirstName, user.LastName, user.Email, user.Role);
            }
            else
            {
                _logger.LogWarning("❌ Login failed - Invalid password for: {Email}", email);
            }
            
            return isValidPassword;
        }

        public async Task<bool> CanCreateAdminAsync(int currentUserId)
        {
            var user = await GetUserByIdAsync(currentUserId);
            return user?.Role == "Admin";
        }

        public async Task<bool> PromoteToAdminAsync(int targetUserId, int currentAdminId)
        {
            // Check if current user is admin
            var currentAdmin = await GetUserByIdAsync(currentAdminId);
            if (currentAdmin?.Role != "Admin")
            {
                return false;
            }

            // Get target user
            var targetUser = await GetUserByIdAsync(targetUserId);
            if (targetUser == null)
            {
                return false;
            }

            // Update role to admin
            targetUser.Role = "Admin";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId)
        {
            // Check if current user is admin
            var currentAdmin = await GetUserByIdAsync(currentAdminId);
            if (currentAdmin?.Role != "Admin")
            {
                return false;
            }

            // Validate new role
            if (!new[] { "Student", "Tutor", "Admin" }.Contains(newRole))
            {
                return false;
            }

            // Get target user
            var targetUser = await GetUserByIdAsync(userId);
            if (targetUser == null)
            {
                return false;
            }

            // Update role
            targetUser.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
