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

        public async Task<IActionResult> OnGetAsync(int? moduleId)
        {
            ForumPosts = await _context.ForumPosts
                .Include(f => f.User)
                .Include(f => f.Comments)
                    .ThenInclude(c => c.User)
                .OrderByDescending(f => f.CreationDate)
                .ToListAsync();

            IQueryable<ForumPost> query = _context.ForumPosts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User);

            if (moduleId.HasValue)
                query = query.Where(p => p.StudentModule.ModuleID == moduleId.Value);

            ForumPosts = await query
                .OrderByDescending(p => p.CreationDate)
                .ToListAsync();

            Modules = await _context.Modules.ToListAsync();
            SelectedModuleId = moduleId;
            return Page();
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
        public async Task<IActionResult> OnPostUpvotePostAsync(int postId)
        {
            var post = await _context.ForumPosts.FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null) return NotFound();

            post.Upvotes += 1;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { moduleId = SelectedModuleId });
        }

        public async Task<IActionResult> OnPostUpvoteCommentAsync(int commentId)
        {
            var comment = await _context.ForumComments.FirstOrDefaultAsync(c => c.CommentID == commentId);
            if (comment == null) return NotFound();

            comment.Upvotes += 1;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { moduleId = SelectedModuleId });
        }


        // Properties for Razor
        public List<Module> Modules { get; set; } = new();
        public int? SelectedModuleId { get; set; }

    }
}
