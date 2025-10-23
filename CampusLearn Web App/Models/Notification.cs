using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn_Web_App.Models
{
    [Table("notification")]
    public class Notification
    {
        [Key]
        [Column("notificationid")]
        public int NotificationID { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("isread")]
        public bool IsRead { get; set; } = false;

        [Required]
        [Column("userid")]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
    }
}
