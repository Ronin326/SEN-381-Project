using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;

public class ForumCreateModel : PageModel
{
    private readonly CampusLearnDbContext _context;

    [BindProperty]
    public ForumPost Post { get; set; }

    public ForumCreateModel(CampusLearnDbContext context)
    {
        _context = context;
    }

    public void OnGet() {}

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        Post.UserID = 1; // For now hardcode a test user
        Post.StudentModuleID = 1;
        Post.CreationDate = DateTime.UtcNow;

        _context.ForumPosts.Add(Post);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Forum/PublicForum");
    }
}
