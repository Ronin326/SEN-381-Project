using System.ComponentModel.DataAnnotations;

namespace CampusLearn_Web_App.Models
{
    public class ForumComment
    {
        public int CommentID { get; set; }

        // Foreign Keys
        public int UserID { get; set; }
        public User? User { get; set; }

        public int PostID { get; set; }
        public ForumPost? ForumPost { get; set; }

        // Optional nested replies
        public int? ParentCommentID { get; set; }
        public ForumComment? ParentComment { get; set; }
        public ICollection<ForumComment> Replies { get; set; } = new List<ForumComment>();

        // Core Content
        [Required]
        public string Comment { get; set; } = string.Empty;

        // Meta Data
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
