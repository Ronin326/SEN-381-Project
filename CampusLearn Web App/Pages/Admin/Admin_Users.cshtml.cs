using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn_Web_App.Pages.Admin
{
	public class Admin_UsersModel : PageModel
	{
		private readonly CampusLearnDbContext _context;

		public Admin_UsersModel(CampusLearnDbContext context)
		{
			_context = context;
		}

		public List<User> Users { get; set; } = new();
		[BindProperty(SupportsGet = true)]
		public string? SearchTerm { get; set; }

		public async Task OnGetAsync()
		{
			IQueryable<User> query = _context.Users;

			if (!string.IsNullOrWhiteSpace(SearchTerm))
			{
				var lower = SearchTerm.ToLower();
				query = query.Where(u =>
					u.FirstName.ToLower().Contains(lower) ||
					u.LastName.ToLower().Contains(lower) ||
					u.Email.ToLower().Contains(lower) ||
					u.Role.ToLower().Contains(lower)
				);
			}

			Users = await query.OrderBy(u => u.UserID).ToListAsync();
		}
	}
}
