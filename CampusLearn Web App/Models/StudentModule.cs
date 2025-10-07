using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn_Web_App.Models
{
    [Table("studentmodule")]
    public class StudentModule
    {
        [Key]
        [Column("studentmoduleid")]
        public int StudentModuleID { get; set; }
        
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
    }
}
