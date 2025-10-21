using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Models;

namespace CampusLearn_Web_App.Data
{
    public class CampusLearnDbContext : DbContext
    {
        public CampusLearnDbContext(DbContextOptions<CampusLearnDbContext> options) : base(options) { }

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

            // -----------------------
            // User
            // -----------------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserID).HasColumnName("userid");
                entity.Property(e => e.FirstName).HasColumnName("firstname").IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).HasColumnName("lastname").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).HasColumnName("passwordhash").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).HasColumnName("role").IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // -----------------------
            // Module
            // -----------------------
            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("module");
                entity.HasKey(e => e.ModuleID);
                entity.Property(e => e.ModuleID).HasColumnName("moduleid");
                entity.Property(e => e.ModuleName).HasColumnName("modulename").IsRequired().HasMaxLength(100);
                entity.Property(e => e.ModuleCode).HasColumnName("modulecode").IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.ModuleCode).IsUnique();
            });

            // -----------------------
            // StudentModule
            // -----------------------
            modelBuilder.Entity<StudentModule>(entity =>
            {
                entity.ToTable("studentmodule");
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

            // -----------------------
            // TutorModule
            // -----------------------
            modelBuilder.Entity<TutorModule>(entity =>
            {
                entity.ToTable("tutormodule");
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

            // -----------------------
            // Topic
            // -----------------------
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

            // -----------------------
            // LearningMaterial
            // -----------------------
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

            // -----------------------
            // Message
            // -----------------------
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message"); // keep consistent with your schema
                entity.HasKey(e => e.MessageID);
                entity.Property(e => e.MessageID).HasColumnName("messageid");
                entity.Property(e => e.SenderID).HasColumnName("senderid");
                entity.Property(e => e.ReceiverID).HasColumnName("receiverid");
                entity.Property(e => e.Content).HasColumnName("content").IsRequired();

                // Default timestamp from DB (Postgres)
                entity.Property(e => e.SentDate)
                      .HasColumnName("sentdate")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(e => e.SenderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(e => e.ReceiverID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ? Helpful indexes for chat queries
                entity.HasIndex(e => new { e.SenderID, e.ReceiverID, e.SentDate });
                entity.HasIndex(e => new { e.ReceiverID, e.SenderID, e.SentDate });
            });

            // -----------------------
            // Notification
            // -----------------------
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
