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
        
        [MaxLength(150)]
        [Column("filename")]
        public string? FileName { get; set; }
        
        [MaxLength(50)]
        [Column("filetype")]
        public string? FileType { get; set; }
        
        [Column("uploaddate")]
        public DateTime? UploadDate { get; set; }
        
        [Required]
        [Column("topicid")]
        public int TopicID { get; set; }
        
        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic Topic { get; set; } = null!;
    }
}
