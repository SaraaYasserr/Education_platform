using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.DTOs
{
    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int LessonOrder { get; set; }
        public int DurationMinutes { get; set; }
        public int CourseId { get; set; }
    }

    public class LessonCreateUpdateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? VideoUrl { get; set; }

        [Range(1, 1000)]
        public int LessonOrder { get; set; }

        [Range(0, 600)]
        public int DurationMinutes { get; set; }

        [Required]
        public int CourseId { get; set; }
    }
}
