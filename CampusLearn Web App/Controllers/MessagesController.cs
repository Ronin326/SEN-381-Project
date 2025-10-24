using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;

namespace CampusLearn_Web_App.Controllers
{
	[Route("api/messages")]
	[ApiController]
	public class MessagesController : ControllerBase
	{
		private readonly CampusLearnDbContext _context;
		public MessagesController(CampusLearnDbContext context)
		{
			_context = context;
		}

		[HttpGet("{senderId:int}/{receiverId:int}")]
		public async Task<IActionResult> GetMessages(int senderId, int receiverId)
		{
			var messages = await _context.Messages
				.Where(m =>
					(m.SenderID == senderId && m.ReceiverID == receiverId) ||
					(m.SenderID == receiverId && m.ReceiverID == senderId))
				.OrderBy(m => m.SentDate)
				.ToListAsync();

			return Ok(messages);
		}

		[HttpPost("send")]
		public async Task<IActionResult> SendMessage([FromBody] Message message)
		{
			if (message == null || string.IsNullOrWhiteSpace(message.Content))
				return BadRequest("Invalid message");

			message.SentDate = DateTime.UtcNow;
			_context.Messages.Add(message);
			await _context.SaveChangesAsync();

			return Ok(message);
		}
	}
}
