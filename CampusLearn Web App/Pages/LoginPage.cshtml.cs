using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusLearn_Web_App.Pages
{
    public class LoginPageModel : PageModel
	{
		[BindProperty]
		public string Email { get; set; }

		[BindProperty]
		public string Password { get; set; }

		public void OnGet()
		{
		}

		public IActionResult OnPost()
		{
			// Fake login for now
			if (Email == "test@example.com" && Password == "1234")
			{
				return RedirectToPage("/LandingPage"); // ✅ go to LandingPage after login
			}

			ModelState.AddModelError("", "Invalid login attempt");
			return Page();
		}
	}
}
