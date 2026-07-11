using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationalPlatform.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Total course duration, stored in minutes for easy aggregation/display.</summary>
        public int DurationMinutes { get; set; }

        /// <summary>Number of seats available; null = unlimited.</summary>
        public int? SeatsAvailable { get; set; }

        public bool IsPublished { get; set; } = true;

        // Foreign keys
        public int CategoryId { get; set; }
        public string InstructorId { get; set; } = string.Empty;

        // Navigation properties
        public Category Category { get; set; } = null!;
        public ApplicationUser Instructor { get; set; } = null!;

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        // Convenience, computed-on-read properties (not mapped to DB)
        [NotMapped]
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;

        [NotMapped]
        public int ReviewCount => Reviews.Count;

        [NotMapped]
        public int EnrollmentCount => Enrollments.Count;
    }
}
