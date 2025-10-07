using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn_Web_App.Models
{
    [Table("topic")]
    public class Topic
    {
        [Key]
        [Column("topicid")]
        public int TopicID { get; set; }
        
        [Required]
        [MaxLength(150)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;
        
        [Column("description")]
        public string? Description { get; set; }
        
        [Required]
        [Column("creationdate")]
        public DateTime CreationDate { get; set; }
        
        [Required]
        [Column("userid")]
        public int UserID { get; set; }
        
        [Required]
        [Column("moduleid")]
        public int ModuleID { get; set; }
        
        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
        
        [ForeignKey("ModuleID")]
        public virtual Module Module { get; set; } = null!;
        
        public virtual ICollection<LearningMaterial> LearningMaterials { get; set; } = new List<LearningMaterial>();
    }
}
