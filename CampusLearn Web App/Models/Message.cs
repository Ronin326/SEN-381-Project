using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn_Web_App.Models
{
    [Table("message")]
    public class Message
    {
        [Key]
        [Column("messageid")]
        public int MessageID { get; set; }
        
        [Required]
        [Column("senderid")]
        public int SenderID { get; set; }
        
        [Required]
        [Column("receiverid")]
        public int ReceiverID { get; set; }
        
        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;
        
        [Column("sentdate")]
        public DateTime SentDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("SenderID")]
        public virtual User Sender { get; set; } = null!;
        
        [ForeignKey("ReceiverID")]
        public virtual User Receiver { get; set; } = null!;
    }
}
