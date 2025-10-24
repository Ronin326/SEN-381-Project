using CampusLearn_Web_App.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CampusLearn_Web_App.Models
{
	public class Admin : User
	{
		// Admin-specific methods
		public static async Task ChangeRoleAsync(CampusLearnDbContext _dbContext, int userId, string newRole)
		{
			// 1. Find the user by their ID
			var userToUpdate = await _dbContext.Users.FindAsync(userId);

			// 2. Check if the user exists before trying to update.
			if (userToUpdate != null)
			{
				// Simple validation to ensure the role is one of the accepted values
				if (newRole == "Student" || newRole == "Tutor" || newRole == "Admin")
				{
					// 3. Update the user's role
					userToUpdate.Role = newRole;

					// 4. Save the changes to the database asynchronously
					await _dbContext.SaveChangesAsync();
				}
				else
				{
					// Handle case where the newRole is invalid
					throw new ArgumentException("Invalid role specified. Role must be 'Student', 'Tutor', or 'Admin'.");
				}
			}
			else
			{
				// Handle case where the user was not found
				throw new InvalidOperationException($"User with ID {userId} not found.");
			}
		}

		public static async Task MakeAnnouncement(CampusLearnDbContext _dbContext, string messageContent)
		{
			// 1. Get all users from the database.
			var allUsers = await _dbContext.Users.ToListAsync();

			// 2. Create a notification for each user.
			var notifications = new List<Notification>();
			foreach (var user in allUsers)
			{
				notifications.Add(new Notification
				{
					Message = messageContent,
					UserID = user.UserID,
					CreatedAt = DateTime.UtcNow
				});
			}

			// 3. Add all new notifications to the database and save changes.
			_dbContext.Notifications.AddRange(notifications);
			await _dbContext.SaveChangesAsync();
		}
	}
}