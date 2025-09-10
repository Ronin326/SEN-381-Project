using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CampusLearn_Web_App.Services;
using CampusLearn_Web_App.Attributes;

namespace CampusLearn_Web_App.Pages
{
    public class RegisterPageModel : PageModel
	{
		private readonly IUserService _userService;

		public RegisterPageModel(IUserService userService)
		{
			_userService = userService;
		}

		[BindProperty]
		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = string.Empty;

		[BindProperty]
		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; } = string.Empty;

		[BindProperty]
		[Required]
		[EmailAddress]
		[BelgiumCampusEmail] // Domain restriction validation
		public string Email { get; set; } = string.Empty;

		[BindProperty]
		[Required]
		[DataType(DataType.Password)]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
		public string Password { get; set; } = string.Empty;

		[BindProperty]
		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; } = string.Empty;

		[BindProperty]
		[Required]
		[RegistrationRole] // This prevents admin role selection
		public string Role { get; set; } = "Student"; // Default to Student

		public string? ErrorMessage { get; set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				// The UserService will automatically reject admin role creation
				var success = await _userService.RegisterUserAsync(FirstName, LastName, Email, Password, Role);
				
				if (!success)
				{
					ErrorMessage = "Registration failed. Email may already be in use.";
					return Page();
				}

				// Redirect to login with success message
				TempData["SuccessMessage"] = "Registration successful! Please log in.";
				return RedirectToPage("/LoginPage");
			}
			catch (UnauthorizedAccessException ex)
			{
				ErrorMessage = ex.Message;
				return Page();
			}
			catch (ArgumentException ex)
			{
				ErrorMessage = ex.Message;
				return Page();
			}
			catch (Exception)
			{
				ErrorMessage = "An error occurred during registration. Please try again.";
				return Page();
			}
		}
	}
}
