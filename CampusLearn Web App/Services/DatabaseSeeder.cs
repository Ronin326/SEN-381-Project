using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn_Web_App.Services
{
	public interface IDatabaseSeeder
	{
		Task SeedAsync();
	}

	public class DatabaseSeeder : IDatabaseSeeder
	{
		private readonly CampusLearnDbContext _context;
		private readonly IPasswordService _passwordService;
		private readonly IConfiguration _configuration;
		private readonly ILogger<DatabaseSeeder> _logger;

		public DatabaseSeeder(CampusLearnDbContext context, IPasswordService passwordService, IConfiguration configuration, ILogger<DatabaseSeeder> logger)
		{
			_context = context;
			_passwordService = passwordService;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task SeedAsync()
		{
			await SeedDefaultAdminAsync();
			await SeedSampleDataAsync();
		}

		private async Task SeedDefaultAdminAsync()
		{
			_logger.LogInformation("🔍 Checking for existing admin users...");

			var adminExists = await _context.Users.AnyAsync(u => u.Role == "Admin");

			if (!adminExists)
			{
				var defaultAdminEmail = _configuration["AdminSettings:DefaultAdminEmail"] ?? "admin@belgiumcampus.ac.za";
				var defaultAdminPassword = _configuration["AdminSettings:DefaultAdminPassword"] ?? "Admin@123456";

				_logger.LogInformation("👑 No admin found. Creating default admin user...");

				var admin = new User
				{
					FirstName = "System",
					LastName = "Administrator",
					Email = defaultAdminEmail,
					PasswordHash = _passwordService.HashPassword(defaultAdminPassword),
					Role = "Admin"
				};

				_context.Users.Add(admin);
				await _context.SaveChangesAsync();

				_logger.LogInformation("✅ Default admin created successfully!");
				_logger.LogInformation("📧 Admin Email: {Email}", defaultAdminEmail);
				_logger.LogInformation("🔑 Admin Password: {Password}", defaultAdminPassword);
				_logger.LogWarning("⚠️ IMPORTANT: Please change the admin password after first login!");
			}
			else
			{
				_logger.LogInformation("✅ Admin user already exists - skipping creation");
			}
		}

		private async Task SeedSampleDataAsync()
		{
			// --- Create Demo Accounts ---
			var demoAccounts = new[]
			{
				new { Email = "demo.admin@belgiumcampus.ac.za", FirstName = "Demo", LastName = "Admin", Role = "Admin", Password = "Admin" },
				new { Email = "demo.tutor@belgiumcampus.ac.za", FirstName = "Demo", LastName = "Tutor", Role = "Tutor", Password = "Tutor" },
				new { Email = "demo.student@belgiumcampus.ac.za", FirstName = "Demo", LastName = "Student", Role = "Student", Password = "Student" }
			};

			foreach (var account in demoAccounts)
			{
				if (!await _context.Users.AnyAsync(u => u.Email == account.Email))
				{
					_context.Users.Add(new User
					{
						Email = account.Email,
						FirstName = account.FirstName,
						LastName = account.LastName,
						Role = account.Role,
						PasswordHash = _passwordService.HashPassword(account.Password)
					});

					_logger.LogInformation("✨ Created demo account - {Role}: {Email} (Password: {Password})",
						account.Role, account.Email, account.Password);
				}
			}

			await _context.SaveChangesAsync();

			// --- Ensure Modules Exist ---
			if (!await _context.Modules.AnyAsync())
			{
				var modules = new[]
				{
					new Module { ModuleName = "Computer Science Fundamentals", ModuleCode = "CS101" },
					new Module { ModuleName = "Database Systems", ModuleCode = "CS201" },
					new Module { ModuleName = "Web Development", ModuleCode = "CS301" },
					new Module { ModuleName = "Software Engineering", ModuleCode = "CS401" },
					new Module { ModuleName = "Data Structures", ModuleCode = "CS202" }
				};

				_context.Modules.AddRange(modules);
				await _context.SaveChangesAsync();
			}

			// --- Add Sample Users ---
			if (!await _context.Users.AnyAsync(u => u.Email.Contains("@belgiumcampus.ac.za") && u.Role == "Tutor"))
			{
				var users = new[]
				{
					new User { FirstName = "John", LastName = "Smith", Email = "john.student@belgiumcampus.ac.za", PasswordHash = _passwordService.HashPassword("Student123!"), Role = "Student" },
					new User { FirstName = "Sarah", LastName = "Johnson", Email = "sarah.student@belgiumcampus.ac.za", PasswordHash = _passwordService.HashPassword("Student123!"), Role = "Student" },
					new User { FirstName = "Mike", LastName = "Wilson", Email = "mike.student@belgiumcampus.ac.za", PasswordHash = _passwordService.HashPassword("Student123!"), Role = "Student" },
					new User { FirstName = "Dr. Alice", LastName = "Brown", Email = "alice.tutor@belgiumcampus.ac.za", PasswordHash = _passwordService.HashPassword("Tutor123!"), Role = "Tutor" },
					new User { FirstName = "Prof. Bob", LastName = "Davis", Email = "bob.tutor@belgiumcampus.ac.za", PasswordHash = _passwordService.HashPassword("Tutor123!"), Role = "Tutor" }
				};

				_context.Users.AddRange(users);
				await _context.SaveChangesAsync();
			}

			// --- Get Data for Relationships ---
			var allModules = await _context.Modules.ToListAsync();
			var allUsers = await _context.Users.ToListAsync();

			var john = allUsers.FirstOrDefault(u => u.Email == "john.student@belgiumcampus.ac.za");
			var sarah = allUsers.FirstOrDefault(u => u.Email == "sarah.student@belgiumcampus.ac.za");
			var mike = allUsers.FirstOrDefault(u => u.Email == "mike.student@belgiumcampus.ac.za");
			var alice = allUsers.FirstOrDefault(u => u.Email == "alice.tutor@belgiumcampus.ac.za");
			var bob = allUsers.FirstOrDefault(u => u.Email == "bob.tutor@belgiumcampus.ac.za");

			// --- Student-Module Enrollments ---
			var studentModules = new[]
			{
				new StudentModule { UserID = john!.UserID, ModuleID = allModules[0].ModuleID },
				new StudentModule { UserID = john.UserID, ModuleID = allModules[1].ModuleID },
				new StudentModule { UserID = sarah!.UserID, ModuleID = allModules[0].ModuleID },
				new StudentModule { UserID = sarah.UserID, ModuleID = allModules[2].ModuleID },
				new StudentModule { UserID = mike!.UserID, ModuleID = allModules[1].ModuleID },
				new StudentModule { UserID = mike.UserID, ModuleID = allModules[3].ModuleID },

                // ✅ Add sample modules for test student (UserID = 10)
                new StudentModule { UserID = 10, ModuleID = allModules[0].ModuleID },
				new StudentModule { UserID = 10, ModuleID = allModules[1].ModuleID },
				new StudentModule { UserID = 10, ModuleID = allModules[2].ModuleID },
				new StudentModule { UserID = 10, ModuleID = allModules[3].ModuleID },
				new StudentModule { UserID = 10, ModuleID = allModules[4].ModuleID }
			};

			_context.StudentModules.AddRange(studentModules);
			await _context.SaveChangesAsync();

			// --- Tutor-Module Assignments ---
			var tutorModules = new[]
			{
				new TutorModule { UserID = alice!.UserID, ModuleID = allModules[0].ModuleID },
				new TutorModule { UserID = alice.UserID, ModuleID = allModules[1].ModuleID },
				new TutorModule { UserID = bob!.UserID, ModuleID = allModules[2].ModuleID },
				new TutorModule { UserID = bob.UserID, ModuleID = allModules[3].ModuleID }
			};

			_context.TutorModules.AddRange(tutorModules);
			await _context.SaveChangesAsync();

			// --- Topics ---
			var topics = new[]
			{
				new Topic { Title = "Introduction to Programming", Description = "Basic programming concepts", CreationDate = DateTime.Now.AddDays(-30), UserID = alice!.UserID, ModuleID = allModules[0].ModuleID },
				new Topic { Title = "SQL Basics", Description = "Introduction to SQL queries", CreationDate = DateTime.Now.AddDays(-25), UserID = alice.UserID, ModuleID = allModules[1].ModuleID },
				new Topic { Title = "HTML & CSS", Description = "Web page structure and styling", CreationDate = DateTime.Now.AddDays(-20), UserID = bob!.UserID, ModuleID = allModules[2].ModuleID },
				new Topic { Title = "JavaScript Fundamentals", Description = "Client-side scripting basics", CreationDate = DateTime.Now.AddDays(-15), UserID = bob.UserID, ModuleID = allModules[2].ModuleID }
			};

			_context.Topics.AddRange(topics);
			await _context.SaveChangesAsync();

			// --- Notifications ---
			var notifications = new[]
			{
				new Notification { Message = "Welcome to CampusLearn!", CreatedAt = DateTime.Now.AddDays(-10), UserID = john.UserID },
				new Notification { Message = "New topic posted in Web Development.", CreatedAt = DateTime.Now.AddDays(-5), UserID = sarah.UserID },
				new Notification { Message = "Your assignment is due next week.", CreatedAt = DateTime.Now.AddDays(-2), UserID = mike.UserID }
			};

			_context.Notifications.AddRange(notifications);
			await _context.SaveChangesAsync();

			Console.WriteLine("✅ Sample data seeded successfully!");
			Console.WriteLine("📚 Students: john.student@belgiumcampus.ac.za, sarah.student@belgiumcampus.ac.za, mike.student@belgiumcampus.ac.za (Password: Student123!)");
			Console.WriteLine("👩‍🏫 Tutors: alice.tutor@belgiumcampus.ac.za, bob.tutor@belgiumcampus.ac.za (Password: Tutor123!)");
		}
	}
}
