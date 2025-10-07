using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn_Web_App.Models
{
    [Table("module")]
    public class Module
    {
        [Key]
        [Column("moduleid")]
        public int ModuleID { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Column("modulename")]
        public string ModuleName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        [Column("modulecode")]
        public string ModuleCode { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ICollection<StudentModule> StudentModules { get; set; } = new List<StudentModule>();
        public virtual ICollection<TutorModule> TutorModules { get; set; } = new List<TutorModule>();
        public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
