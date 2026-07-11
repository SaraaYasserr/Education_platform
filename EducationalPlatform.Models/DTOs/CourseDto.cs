using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DurationMinutes { get; set; }
        public int? SeatsAvailable { get; set; }
        public bool IsPublished { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string InstructorId { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int EnrollmentCount { get; set; }
    }

    public class CourseCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0, 100000)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string InstructorId { get; set; } = string.Empty;

        [Range(0, 10000)]
        public int DurationMinutes { get; set; }

        public int? SeatsAvailable { get; set; }

        public bool IsPublished { get; set; } = true;
    }

    public class CourseUpdateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0, 100000)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Range(0, 10000)]
        public int DurationMinutes { get; set; }

        public int? SeatsAvailable { get; set; }

        public bool IsPublished { get; set; }
    }
}
