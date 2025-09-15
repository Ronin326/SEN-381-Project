using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn_Web_App.Models
{
    [Table("learningmaterial")]
    public class LearningMaterial
    {
        [Key]
        [Column("materialid")]
        public int MaterialID { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("filetype")]
        public string FileType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("filepath")]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [Column("uploaddate")]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("adminid")]
        public int AdminID { get; set; } // AdminID

        [Required]
        [Column("moduleid")]
        public int ModuleID { get; set; } // Links to Module

        [Required]
        [Column("topicid")]
        public int TopicID { get; set; }

        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic Topic { get; set; } = null!;
        
        // Methods (logic in services/controllers)
        public void Upload()
        {
            // Implementation in service/controller
        }

        public void Download()
        {
            // Implementation in service/controller
        }

        public void View()
        {
            // Implementation in service/controller
        }
    }
}
