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
        [MaxLength(20)]
        [Column("modulecode")]
        public string ModuleCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("modulename")]
        public string ModuleName { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Active"; // is active or not active.

        [Required]
        [Column("createdby")]
        public int CreatedBy { get; set; } // This would be Tutor UserID

        public string DisplayLabel() => $"{ModuleCode} - {ModuleName}";

        // Methods (logic will be in services or controllers)
        public void AddStudent(int studentId)
        {
            // Implementation in service/controller
        }

        public void RemoveStudent(int studentId)
        {
            // Implementation in service/controller
        }

        public void ListMaterials()
        {
            // Implementation in service/controller
        }
        // Navigation properties
        public virtual ICollection<StudentModule> StudentModules { get; set; } = new List<StudentModule>();
        public virtual ICollection<TutorModule> TutorModules { get; set; } = new List<TutorModule>();
        public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
