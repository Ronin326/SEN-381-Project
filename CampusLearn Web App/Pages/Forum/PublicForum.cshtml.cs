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

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string Content { get; set; }

        public async Task OnGetAsync()
        {
            ForumPosts = await _context.ForumPosts
                .Include(f => f.User)
                .Include(f => f.Comments)
                    .ThenInclude(c => c.User)
                .OrderByDescending(f => f.CreationDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
                return RedirectToPage();

            var newPost = new ForumPost
            {
                Title = Title,
                Content = Content,
                CreationDate = DateTime.UtcNow,
                UserID = 1 // change this later for logged-in user
            };

            _context.ForumPosts.Add(newPost);
            await _context.SaveChangesAsync();

            return RedirectToPage(); // forces a reload of posts
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int PostId, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
                return RedirectToPage();

            var comment = new ForumComment
            {
                PostID = PostId,
                Comment = Content,
                CreationDate = DateTime.UtcNow,
                UserID = 1 // placeholder user
            };

            _context.ForumComments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
