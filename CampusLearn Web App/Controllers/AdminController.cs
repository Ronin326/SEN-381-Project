using Microsoft.AspNetCore.Mvc;
using CampusLearn_Web_App.Services;
using CampusLearn_Web_App.Filters;
using CampusLearn_Web_App.Extensions;

namespace CampusLearn_Web_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;

        public AdminController(IAdminService adminService, IUserService userService)
        {
            _adminService = adminService;
            _userService = userService;
        }

        [HttpGet("users")]
        [RequireAdmin]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                var userList = users.Select(u => new
                {
                    u.UserID,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role
                });
                
                return Ok(userList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching users.", error = ex.Message });
            }
        }

        [HttpPost("users/{userId}/role")]
        [RequireAdmin]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetCurrentUserId();
                if (!currentUserId.HasValue)
                    return Unauthorized();

                var success = await _adminService.UpdateUserRoleAsync(userId, request.NewRole, currentUserId.Value);
                
                if (success)
                    return Ok(new { message = "User role updated successfully." });
                else
                    return BadRequest(new { message = "Failed to update user role." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating user role.", error = ex.Message });
            }
        }

        [HttpPost("users")]
        [RequireAdmin]
        public async Task<IActionResult> CreateAdminUser([FromBody] CreateAdminRequest request)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetCurrentUserId();
                if (!currentUserId.HasValue)
                    return Unauthorized();

                var success = await _adminService.CreateAdminUserAsync(
                    request.FirstName, 
                    request.LastName, 
                    request.Email, 
                    request.Password, 
                    currentUserId.Value);

                if (success)
                    return Ok(new { message = "Admin user created successfully." });
                else
                    return BadRequest(new { message = "Failed to create admin user. Email may already exist." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating admin user.", error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        [RequireAdmin]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var stats = await _adminService.GetUserStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching statistics.", error = ex.Message });
            }
        }

        [HttpDelete("users/{userId}")]
        [RequireAdmin]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetCurrentUserId();
                if (!currentUserId.HasValue)
                    return Unauthorized();

                var success = await _adminService.DeleteUserAsync(userId, currentUserId.Value);
                
                if (success)
                    return Ok(new { message = "User deleted successfully." });
                else
                    return BadRequest(new { message = "Failed to delete user." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting user.", error = ex.Message });
            }
        }
    }

    // DTOs for API requests
    public class UpdateRoleRequest
    {
        public string NewRole { get; set; } = string.Empty;
    }

    public class CreateAdminRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
