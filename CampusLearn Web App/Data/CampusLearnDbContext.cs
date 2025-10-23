using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Models;

namespace CampusLearn_Web_App.Data
{
    public class CampusLearnDbContext : DbContext
    {
        public CampusLearnDbContext(DbContextOptions<CampusLearnDbContext> options) : base(options)
        {
        }
		public async Task SeedAsync()
		{
			// Ensure the database exists
			await this.Database.EnsureCreatedAsync();

			// Check if there are already any modules in the database
			if (!this.Modules.Any())
			{
				// ==============================
				// ? Seed dummy Module records
				// ==============================
				var modules = new List<Module>
		        {
			        new Module { ModuleCode = "ACW181", ModuleName = "Academic Writing 181" },
			        new Module { ModuleCode = "COA181", ModuleName = "Computer Architecture 181" },
			        new Module { ModuleCode = "DBD181", ModuleName = "Database Development 181" },
			        new Module { ModuleCode = "INF181", ModuleName = "Information Systems 181" },
			        new Module { ModuleCode = "INL101", ModuleName = "Innovation and Leadership 101" },
			        new Module { ModuleCode = "INL102", ModuleName = "Innovation and Leadership 102" },
			        new Module { ModuleCode = "LPR181", ModuleName = "Linear Programming 181" },
			        new Module { ModuleCode = "MAT181", ModuleName = "Mathematics 181" },
			        new Module { ModuleCode = "NWD181", ModuleName = "Networking Development 181" },
			        new Module { ModuleCode = "PRG181", ModuleName = "Programming 181" },
			        new Module { ModuleCode = "PRG182", ModuleName = "Programming 182" },
			        new Module { ModuleCode = "STA181", ModuleName = "Statistics 181" },
			        new Module { ModuleCode = "WPR181", ModuleName = "Web Programming 181" },
			        new Module { ModuleCode = "BUM181", ModuleName = "Business Management 181" },
			        new Module { ModuleCode = "ENT181", ModuleName = "Entrepreneurship 181" },
			        new Module { ModuleCode = "DBD281", ModuleName = "Database Development 281" },
			        new Module { ModuleCode = "INF281", ModuleName = "Information Systems 281" },
			        new Module { ModuleCode = "INL201", ModuleName = "Innovation and Leadership 201" },
			        new Module { ModuleCode = "INL202", ModuleName = "Innovation and Leadership 202" },
			        new Module { ModuleCode = "LPR281", ModuleName = "Linear Programming 281" },
			        new Module { ModuleCode = "MAT281", ModuleName = "Mathematics 281" },
			        new Module { ModuleCode = "PRG281", ModuleName = "Programming 281" },
			        new Module { ModuleCode = "PRG282", ModuleName = "Programming 282" },
			        new Module { ModuleCode = "PMM281", ModuleName = "Project Management 281" },
			        new Module { ModuleCode = "STA281", ModuleName = "Statistics 281" },
			        new Module { ModuleCode = "WPR281", ModuleName = "Web Programming 281" },
			        new Module { ModuleCode = "SAD281", ModuleName = "Software Analysis & Design 281" },
			        new Module { ModuleCode = "DWH281", ModuleName = "Data Warehousing 281" },
			        new Module { ModuleCode = "IOT281", ModuleName = "Internet Of Things 281" },
			        new Module { ModuleCode = "SWT281", ModuleName = "Software Testing 281" },
			        new Module { ModuleCode = "RSH381", ModuleName = "Research Methods 381" },
			        new Module { ModuleCode = "DBD381", ModuleName = "Database Development 381" },
			        new Module { ModuleCode = "INL321", ModuleName = "Innovation and Leadership 321" },
			        new Module { ModuleCode = "LPR381", ModuleName = "Linear Programming 381" },
			        new Module { ModuleCode = "MLG381", ModuleName = "Machine Learning 381" },
			        new Module { ModuleCode = "PRJ381", ModuleName = "Project 381" },
			        new Module { ModuleCode = "PMM381", ModuleName = "Project Management 381" },
			        new Module { ModuleCode = "PRG381", ModuleName = "Programming 381" },
			        new Module { ModuleCode = "SEN381", ModuleName = "Software Engineering 381" },
			        new Module { ModuleCode = "WPR381", ModuleName = "Web Programming 381" },
			        new Module { ModuleCode = "BIN381", ModuleName = "Data Science 381" },
			        new Module { ModuleCode = "DBA381", ModuleName = "Database Administration 381" },
			        new Module { ModuleCode = "STA381", ModuleName = "Statistics 381" },
			        new Module { ModuleCode = "INM381", ModuleName = "Innovation Management 381" },
			        new Module { ModuleCode = "MLG382", ModuleName = "Machine Learning 382" },
			        new Module { ModuleCode = "UAX381", ModuleName = "User Experience Design 381" },
			        new Module { ModuleCode = "AIT481", ModuleName = "Applied Information Technology 481" },
			        new Module { ModuleCode = "AIT482", ModuleName = "Applied Information Technology 482" },
			        new Module { ModuleCode = "DST481", ModuleName = "Dissertation 481" },
			        new Module { ModuleCode = "ACW171", ModuleName = "Academic Writing 171" },
			        new Module { ModuleCode = "COA171", ModuleName = "Computer Architecture 171" },
			        new Module { ModuleCode = "DBD171", ModuleName = "Database Development 171" },
			        new Module { ModuleCode = "ENG171", ModuleName = "English Communication 171" },
			        new Module { ModuleCode = "INF171", ModuleName = "Information Systems 171" },
			        new Module { ModuleCode = "INL111", ModuleName = "Innovation and Leadership 101" },
			        new Module { ModuleCode = "INL112", ModuleName = "Innovation and Leadership 102" },
			        new Module { ModuleCode = "MAT171", ModuleName = "Mathematics 171" },
			        new Module { ModuleCode = "NWD171", ModuleName = "Networking Development 171" },
			        new Module { ModuleCode = "PRG171", ModuleName = "Programming 171" },
			        new Module { ModuleCode = "PRG172", ModuleName = "Programming 172" },
			        new Module { ModuleCode = "STA171", ModuleName = "Statistics 171" },
			        new Module { ModuleCode = "WPR171", ModuleName = "Web Programming 171" },
			        new Module { ModuleCode = "BUM171", ModuleName = "Business Management 171" },
			        new Module { ModuleCode = "ENT171", ModuleName = "Entrepreneurship 171" },
			        new Module { ModuleCode = "CNA271", ModuleName = "Cloud-Native Application Architecture 271" },
			        new Module { ModuleCode = "DBD221", ModuleName = "Database Development 221" },
			        new Module { ModuleCode = "ERP271", ModuleName = "Enterprise Systems 271" },
			        new Module { ModuleCode = "ETH271", ModuleName = "Ethics 271" },
			        new Module { ModuleCode = "INF271", ModuleName = "Information Systems 271" },
			        new Module { ModuleCode = "INL211", ModuleName = "Innovation and Leadership 201" },
			        new Module { ModuleCode = "INL212", ModuleName = "Innovation and Leadership 202" },
			        new Module { ModuleCode = "LPR171", ModuleName = "Linear Programming 171" },
			        new Module { ModuleCode = "PRG271", ModuleName = "Programming 271" },
			        new Module { ModuleCode = "PRG272", ModuleName = "Programming 272" },
			        new Module { ModuleCode = "PMM271", ModuleName = "Project Management 271" },
			        new Module { ModuleCode = "STA271", ModuleName = "Statistics 271" },
			        new Module { ModuleCode = "WPR271", ModuleName = "Web Programming 271" },
			        new Module { ModuleCode = "IOT271", ModuleName = "Internet Of Things 271" },
			        new Module { ModuleCode = "SWT271", ModuleName = "Software Testing 271" },
			        new Module { ModuleCode = "BIN371", ModuleName = "Business Intelligence 371" },
			        new Module { ModuleCode = "CNA371", ModuleName = "Cloud-Native Application Programming 371" },
			        new Module { ModuleCode = "DAL371", ModuleName = "Data Analytics 371" },
			        new Module { ModuleCode = "DBD371", ModuleName = "Database Development 371" },
			        new Module { ModuleCode = "INL371", ModuleName = "Innovation and Leadership 371" },
			        new Module { ModuleCode = "PRG371", ModuleName = "Programming 371" },
			        new Module { ModuleCode = "PRJ371", ModuleName = "Project 371" },
			        new Module { ModuleCode = "PMM371", ModuleName = "Project Management 371" },
			        new Module { ModuleCode = "SAD371", ModuleName = "Software Analysis & Design 371" },
			        new Module { ModuleCode = "SEN371", ModuleName = "Software Engineering 371" },
			        new Module { ModuleCode = "WPR371", ModuleName = "Web Programming 371" },
			        new Module { ModuleCode = "INM371", ModuleName = "Innovation Management 371" },
			        new Module { ModuleCode = "UAX371", ModuleName = "User Experience Design 371" },
			        new Module { ModuleCode = "ENG281", ModuleName = "English 381" }
		        };

				this.Modules.AddRange(modules);
				await this.SaveChangesAsync();
			}
            // ==============================
            // ? Seed dummy Student Module records
            // ==============================
            //var studentModules = new List<StudentModule>
            //{
            //    new StudentModule { UserID = 10, ModuleID = 71 },
            //    new StudentModule { UserID = 10, ModuleID = 72 },
            //    new StudentModule { UserID = 10, ModuleID = 73 },
            //    new StudentModule { UserID = 10, ModuleID = 80 },
            //    new StudentModule { UserID = 10, ModuleID = 81 }
            //};

            //this.StudentModules.AddRange(studentModules);
            //await this.SaveChangesAsync(); 
            var studentModuleCount = await this.StudentModules.CountAsync();
			Console.WriteLine($"? Seed check: Found {studentModuleCount} StudentModules");
		}



		public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<StudentModule> StudentModules { get; set; }
        public DbSet<TutorModule> TutorModules { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<LearningMaterial> LearningMaterials { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User table
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User"); // Match your schema exactly
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserID).HasColumnName("userid");
                entity.Property(e => e.FirstName).HasColumnName("firstname").IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).HasColumnName("lastname").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).HasColumnName("passwordhash").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).HasColumnName("role").IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Module table
            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("module"); // lowercase to match your schema
                entity.HasKey(e => e.ModuleID);
                entity.Property(e => e.ModuleID).HasColumnName("moduleid");
                entity.Property(e => e.ModuleName).HasColumnName("modulename").IsRequired().HasMaxLength(100);
                entity.Property(e => e.ModuleCode).HasColumnName("modulecode").IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.ModuleCode).IsUnique();
            });

            // Configure StudentModule relationships
            modelBuilder.Entity<StudentModule>(entity =>
            {
                entity.ToTable("studentmodule"); // lowercase to match schema
                entity.HasKey(e => e.StudentModuleID);
                entity.Property(e => e.StudentModuleID).HasColumnName("studentmoduleid");
                entity.Property(e => e.UserID).HasColumnName("userid");
                entity.Property(e => e.ModuleID).HasColumnName("moduleid");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.StudentModules)
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Module)
                    .WithMany(m => m.StudentModules)
                    .HasForeignKey(e => e.ModuleID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TutorModule relationships
            modelBuilder.Entity<TutorModule>(entity =>
            {
                entity.ToTable("tutormodule"); // lowercase to match schema
                entity.HasKey(e => e.TutorModuleID);
                entity.Property(e => e.TutorModuleID).HasColumnName("tutormoduleid");
                entity.Property(e => e.UserID).HasColumnName("userid");
                entity.Property(e => e.ModuleID).HasColumnName("moduleid");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.TutorModules)
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Module)
                    .WithMany(m => m.TutorModules)
                    .HasForeignKey(e => e.ModuleID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Topic relationships
            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("Topic");
                entity.HasKey(e => e.TopicID);
                entity.Property(e => e.TopicID).HasColumnName("topicid");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CreationDate).HasColumnName("creationdate").IsRequired();
                entity.Property(e => e.UserID).HasColumnName("userid");
                entity.Property(e => e.ModuleID).HasColumnName("moduleid");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Topics)
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Module)
                    .WithMany(m => m.Topics)
                    .HasForeignKey(e => e.ModuleID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure LearningMaterial relationships
            modelBuilder.Entity<LearningMaterial>(entity =>
            {
                entity.ToTable("LearningMaterial");
                entity.HasKey(e => e.MaterialID);
                entity.Property(e => e.MaterialID).HasColumnName("materialid");
                entity.Property(e => e.FileName).HasColumnName("filename").HasMaxLength(150);
                entity.Property(e => e.FileType).HasColumnName("filetype").HasMaxLength(50);
                entity.Property(e => e.UploadDate).HasColumnName("uploaddate");
                entity.Property(e => e.TopicID).HasColumnName("topicid");
                entity.HasOne(e => e.Topic)
                    .WithMany(t => t.LearningMaterials)
                    .HasForeignKey(e => e.TopicID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Message relationships
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");
                entity.HasKey(e => e.MessageID);
                entity.Property(e => e.MessageID).HasColumnName("messageid");
                entity.Property(e => e.SenderID).HasColumnName("senderid");
                entity.Property(e => e.ReceiverID).HasColumnName("receiverid");
                entity.Property(e => e.Content).HasColumnName("content").IsRequired();
                entity.Property(e => e.SentDate).HasColumnName("sentdate").HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.HasOne(e => e.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(e => e.SenderID)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(e => e.ReceiverID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Notification relationships
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");
                entity.HasKey(e => e.NotificationID);
                entity.Property(e => e.NotificationID).HasColumnName("notificationid");
                entity.Property(e => e.Message).HasColumnName("message").IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserID).HasColumnName("userid");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
