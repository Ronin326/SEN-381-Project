using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn_Web_App.Pages
{
    public class RegisterPageModel : PageModel
	{
		[BindProperty]
		[Required]
		public string FullName { get; set; }

		[BindProperty]
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[BindProperty]
		[Required]
		[Phone]
		public string Phone { get; set; }

		[BindProperty]
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[BindProperty]
		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }

		public void OnGet()
		{
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			// TODO: Save the user to the database here later
			// For now, just redirect to login page after successful "registration"
			return RedirectToPage("/LoginPage");
		}
	}
}
