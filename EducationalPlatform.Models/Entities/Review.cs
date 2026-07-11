using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public int CourseId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
