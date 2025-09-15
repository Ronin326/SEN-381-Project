using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CampusLearn_Web_App.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn_Web_App.Models
{
    [Table("learningmaterial")]
    public class LearningMaterial
    {
        [Key]
        [Column("materialid")]
        public int MaterialID { get; set; }

        [MaxLength(150)]
        [Column("filename")]
        public string? FileName { get; set; }

        [MaxLength(50)]
        [Column("filetype")]
        public string? FileType { get; set; }

        [Column("uploaddate")]
        public DateTime? UploadDate { get; set; }

        [Required]
        [Column("topicid")]
        public int TopicID { get; set; }

        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic Topic { get; set; } = null!;

        // -------------------------
        // CRUD + Download Functions
        // -------------------------
        public static async Task AddLM(CampusLearnDbContext context, LearningMaterial material)
        {
            context.LearningMaterials.Add(material);
            await context.SaveChangesAsync();
        }

        public static async Task UpdateLM(CampusLearnDbContext context, LearningMaterial material)
        {
            context.LearningMaterials.Update(material);
            await context.SaveChangesAsync();
        }

        public static async Task DeleteLM(CampusLearnDbContext context, int id)
        {
            var material = await context.LearningMaterials.FindAsync(id);
            if (material != null)
            {
                context.LearningMaterials.Remove(material);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<(byte[] FileBytes, string FileName, string FileType)?> Download(
            CampusLearnDbContext  context,
            int id,
            string filePathRoot)
        {
            var material = await context.LearningMaterials.FindAsync(id);
            if (material == null || string.IsNullOrEmpty(material.FileName))
                return null;

            string fullPath = Path.Combine(filePathRoot, material.FileName);
            if (!File.Exists(fullPath))
                return null;

            var fileBytes = await File.ReadAllBytesAsync(fullPath);
            return (fileBytes, material.FileName, material.FileType ?? "application/octet-stream");
        }
    }
}
