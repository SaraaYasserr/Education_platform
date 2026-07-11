using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.Entities
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>URL of the lesson video (e.g. a YouTube embed link), optional for text-only lessons.</summary>
        public string? VideoUrl { get; set; }

        /// <summary>Used to order lessons within a course (1, 2, 3, ...).</summary>
        public int LessonOrder { get; set; }

        /// <summary>Estimated lesson length in minutes, shown to students before they start.</summary>
        public int DurationMinutes { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public ICollection<LessonProgress> ProgressRecords { get; set; } = new List<LessonProgress>();
    }
}
