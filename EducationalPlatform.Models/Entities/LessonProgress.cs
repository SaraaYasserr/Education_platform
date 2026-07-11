namespace EducationalPlatform.Models.Entities
{
    /// <summary>
    /// Tracks whether a specific student has completed a specific lesson.
    /// One row per (StudentId, LessonId) pair. The aggregate of these rows
    /// for a course drives Enrollment.ProgressPercentage and, once every
    /// lesson in the course is completed, triggers automatic Certificate
    /// issuance (see LessonProgressService.ToggleLessonCompletionAsync).
    /// </summary>
    public class LessonProgress
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public int LessonId { get; set; }

        /// <summary>Denormalized so "all progress rows for this course" can be queried without joining through Lesson.</summary>
        public int CourseId { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public ApplicationUser Student { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
