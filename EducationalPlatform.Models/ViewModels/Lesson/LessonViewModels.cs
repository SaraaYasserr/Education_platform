using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Lesson
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int LessonOrder { get; set; }
        public int DurationMinutes { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
    }

    public class LessonCreateViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;

        [Required, MaxLength(200), Display(Name = "Lesson Title")]
        public string Title { get; set; } = string.Empty;

        [Required, Display(Name = "Lesson Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Video URL (YouTube embed link, optional)")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? VideoUrl { get; set; }

        [Range(1, 1000), Display(Name = "Order")]
        public int LessonOrder { get; set; } = 1;

        [Range(0, 600), Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }
    }

    public class LessonEditViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;

        [Required, MaxLength(200), Display(Name = "Lesson Title")]
        public string Title { get; set; } = string.Empty;

        [Required, Display(Name = "Lesson Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Video URL (YouTube embed link, optional)")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? VideoUrl { get; set; }

        [Range(1, 1000), Display(Name = "Order")]
        public int LessonOrder { get; set; }

        [Range(0, 600), Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }
    }
}
