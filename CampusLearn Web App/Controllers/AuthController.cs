using Microsoft.AspNetCore.Mvc;
using CampusLearn_Web_App.Services;
using CampusLearn_Web_App.Extensions;

namespace CampusLearn_Web_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("🔐 API Login attempt for: {Email}", request.Email);

            try
            {
                var isValid = await _userService.ValidateUserAsync(request.Email, request.Password);
                
                if (isValid)
                {
                    var user = await _userService.GetUserByEmailAsync(request.Email);
                    if (user != null)
                    {
                        // Set session
                        HttpContext.Session.SetInt32("UserId", user.UserID);
                        HttpContext.Session.SetString("UserEmail", user.Email);
                        HttpContext.Session.SetString("UserRole", user.Role);
                        HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

                        _logger.LogInformation("✅ API Login successful for: {FirstName} {LastName} ({Email})", 
                            user.FirstName, user.LastName, user.Email);

                        return Ok(new
                        {
                            success = true,
                            message = "Login successful",
                            user = new
                            {
                                id = user.UserID,
                                firstName = user.FirstName,
                                lastName = user.LastName,
                                email = user.Email,
                                role = user.Role
                            }
                        });
                    }
                }

                _logger.LogWarning("❌ API Login failed for: {Email}", request.Email);
                return Unauthorized(new { success = false, message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError("💥 API Login error for {Email}: {Error}", request.Email, ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred during login" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("🚀 API Registration attempt for: {Email}", request.Email);

            try
            {
                var success = await _userService.RegisterUserAsync(
                    request.FirstName, 
                    request.LastName, 
                    request.Email, 
                    request.Password, 
                    request.Role ?? "Student");

                if (success)
                {
                    _logger.LogInformation("✅ API Registration successful for: {FirstName} {LastName} ({Email})", 
                        request.FirstName, request.LastName, request.Email);

                    return Ok(new
                    {
                        success = true,
                        message = "Registration successful"
                    });
                }

                return BadRequest(new { success = false, message = "Registration failed - email may already exist" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("🚨 API Registration blocked: {Message} for {Email}", ex.Message, request.Email);
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("❌ API Registration validation failed: {Message} for {Email}", ex.Message, request.Email);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("💥 API Registration error for {Email}: {Error}", request.Email, ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred during registration" });
            }
        }

        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userRole = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");

            if (userId.HasValue)
            {
                _logger.LogInformation("✅ Auth status check - User authenticated: {UserName} ({Email}) - Role: {Role}", 
                    userName, userEmail, userRole);

                return Ok(new
                {
                    authenticated = true,
                    user = new
                    {
                        id = userId.Value,
                        email = userEmail,
                        name = userName,
                        role = userRole
                    }
                });
            }

            _logger.LogInformation("❌ Auth status check - No authenticated user");
            return Ok(new { authenticated = false });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            HttpContext.Session.Clear();

            _logger.LogInformation("👋 Logout successful for: {UserName} ({Email})", userName, userEmail);

            // Check if it's an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Ok(new { success = true, redirectTo = "/LoginPage" });
            }

            // For regular form submit, redirect to login
            return LocalRedirect("/LoginPage");
        }

        [HttpGet("user-count")]
        public async Task<IActionResult> GetUserCount()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var userCount = users.Count();
                var roleStats = users.GroupBy(u => u.Role)
                                    .ToDictionary(g => g.Key, g => g.Count());

                _logger.LogInformation("📊 User statistics requested - Total: {Total}", userCount);

                return Ok(new
                {
                    success = true,
                    totalUsers = userCount,
                    roleBreakdown = roleStats,
                    lastUpdated = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("💥 Error getting user count: {Error}", ex.Message);
                return StatusCode(500, new { success = false, message = "Error getting user statistics" });
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
