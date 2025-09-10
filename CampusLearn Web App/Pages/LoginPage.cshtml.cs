using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CampusLearn_Web_App.Services;
using CampusLearn_Web_App.Attributes;

namespace CampusLearn_Web_App.Pages
{
    public class LoginPageModel : PageModel
	{
		private readonly IUserService _userService;

		public LoginPageModel(IUserService userService)
		{
			_userService = userService;
		}

		[BindProperty]
		[Required]
		[EmailAddress]
		[BelgiumCampusEmail] // Domain restriction validation
		public string Email { get; set; } = string.Empty;

		[BindProperty]
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		public string? ErrorMessage { get; set; }

		public void OnGet()
		{
			// Check for success message from registration
			if (TempData["SuccessMessage"] != null)
			{
				ViewData["SuccessMessage"] = TempData["SuccessMessage"]?.ToString() ?? "";
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			// Additional server-side domain validation
			if (!Email.EndsWith("@belgiumcampus.ac.za", StringComparison.OrdinalIgnoreCase))
			{
				ErrorMessage = "Only Belgium Campus email addresses (@belgiumcampus.ac.za) are allowed.";
				return Page();
			}

			try
			{
				var isValid = await _userService.ValidateUserAsync(Email, Password);
				
				if (isValid)
				{
					var user = await _userService.GetUserByEmailAsync(Email);
					
					if (user != null)
					{
						// Store user info in session
						HttpContext.Session.SetInt32("UserId", user.UserID);
						HttpContext.Session.SetString("UserEmail", user.Email);
						HttpContext.Session.SetString("UserRole", user.Role);
						HttpContext.Session.SetString("UserFirstName", user.FirstName);
						HttpContext.Session.SetString("UserLastName", user.LastName);
						HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

						// Log successful authentication for console debugging
						TempData["LoginSuccess"] = $"Welcome back, {user.FirstName}! Login successful.";
						TempData["UserRole"] = user.Role;
						TempData["LoginTimestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

						// Redirect based on role
						return user.Role switch
						{
							"Admin" => RedirectToPage("/Admin/Dashboard"),
							"Tutor" => RedirectToPage("/Tutor/Dashboard"),
							"Student" => RedirectToPage("/Student/Dashboard"),
							_ => RedirectToPage("/LandingPage")
						};
					}
				}

				ErrorMessage = "Invalid email or password.";
				return Page();
			}
			catch (Exception)
			{
				ErrorMessage = "An error occurred during login. Please try again.";
				return Page();
			}
		}
	}
}
