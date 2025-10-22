using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // <-- add this for async ToListAsync()

namespace CampusLearn_Web_App.Pages.Admin
{
	public class Admin_CoursesModel : PageModel
	{
		private readonly CampusLearnDbContext _context;

		public Admin_CoursesModel(CampusLearnDbContext context)
		{
			_context = context;
		}

		public List<Module> Modules { get; set; } = new();

		public async Task OnGetAsync()
		{
			// ? Fetch all modules from the database
			Modules = await _context.Modules.ToListAsync();
		}
	}
}
