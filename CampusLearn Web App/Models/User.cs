using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn_Web_App.Models
{
	[Table("User")]
	public class User
	{
		[Key]
		[Column("userid")]
		public int UserID { get; set; }

		[Required]
		[MaxLength(50)]
		[Column("firstname")]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[MaxLength(50)]
		[Column("lastname")]
		public string LastName { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		[Column("email")]
		public string Email { get; set; } = string.Empty;

		[Required]
		[MaxLength(255)]
		[Column("passwordhash")]
		public string PasswordHash { get; set; } = string.Empty;

		[Required]
		[MaxLength(10)]
		[Column("role")]
		public string Role { get; set; } = string.Empty;

		// Navigation properties
		public virtual ICollection<StudentModule> StudentModules { get; set; } = new List<StudentModule>();
		public virtual ICollection<TutorModule> TutorModules { get; set; } = new List<TutorModule>();
		public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
		public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
		public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
		public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

		// Methods that apply to all users
		public void Logout(HttpContext httpContext)
		{
			// Clear all session data for the current user.
			httpContext.Session.Clear();
		}
	}
}