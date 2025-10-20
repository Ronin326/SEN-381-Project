using System.ComponentModel.DataAnnotations;

namespace CampusLearn_Web_App.Models
{
    public class ForumPost
    {
        public int PostID { get; set; }

        // Foreign Keys
        public int UserID { get; set; }
        public User? User { get; set; }

        public int StudentModuleID { get; set; }
        public StudentModule? StudentModule { get; set; }

        // Core Content
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        // Meta Data
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ForumComment> Comments { get; set; } = new List<ForumComment>();
    }
}
