using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Models;

namespace CampusLearn_Web_App.Data
{
    public class CampusLearnDbContext : DbContext
    {
        public CampusLearnDbContext(DbContextOptions<CampusLearnDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<StudentModule> StudentModules { get; set; }
        public DbSet<TutorModule> TutorModules { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<LearningMaterial> LearningMaterials { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<ForumComment> ForumComments { get; set; }


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
                entity.ToTable("message");
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

            modelBuilder.Entity<ForumPost>(entity =>
{
    entity.ToTable("forumpost");
    entity.HasKey(p => p.PostID);
    entity.Property(p => p.PostID).HasColumnName("postid");

    entity.HasOne(p => p.User)
        .WithMany()
        .HasForeignKey(p => p.UserID)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(p => p.StudentModule)
        .WithMany()
        .HasForeignKey(p => p.StudentModuleID)
        .OnDelete(DeleteBehavior.Cascade);
});

            modelBuilder.Entity<ForumComment>(entity =>
            {
                entity.ToTable("forumcomment");
                entity.HasKey(c => c.CommentID);
                entity.Property(c => c.CommentID).HasColumnName("commentid");

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ForumPost)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ParentComment)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(c => c.ParentCommentID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
