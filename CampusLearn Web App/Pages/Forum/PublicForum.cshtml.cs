using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;

namespace CampusLearn_Web_App.Pages.Forum
{
    public class PublicForumModel : PageModel
    {
        private readonly CampusLearnDbContext _context;

        public PublicForumModel(CampusLearnDbContext context)
        {
            _context = context;
        }

        public List<ForumPost> ForumPosts { get; set; } = new();
        [BindProperty] public ForumPost NewPost { get; set; } = new();
        [BindProperty] public ForumComment NewComment { get; set; } = new();

        public async Task OnGetAsync()
        {
            ForumPosts = await _context.ForumPosts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreationDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCreatePostAsync()
        {
            if (!ModelState.IsValid) return Page();

            NewPost.CreationDate = DateTime.UtcNow;
            _context.ForumPosts.Add(NewPost);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int postId)
        {
            if (!ModelState.IsValid) return Page();

            NewComment.PostID = postId;
            NewComment.CreationDate = DateTime.UtcNow;
            _context.ForumComments.Add(NewComment);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
