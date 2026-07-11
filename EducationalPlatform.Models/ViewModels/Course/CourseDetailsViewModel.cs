using EducationalPlatform.Models.ViewModels.Lesson;
using EducationalPlatform.Models.ViewModels.Review;

namespace EducationalPlatform.Models.ViewModels.Course
{
    /// <summary>
    /// Backs course_details.html: the Overview/Curriculum/Instructor/Reviews
    /// tabs, the price/seats sidebar, and the related-courses widget.
    /// </summary>
    public class CourseDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DurationMinutes { get; set; }
        public int? SeatsAvailable { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public string InstructorId { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string? InstructorImage { get; set; }
        public string? InstructorBio { get; set; }

        public List<LessonViewModel> Lessons { get; set; } = new();
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public ReviewCreateViewModel NewReview { get; set; } = new();

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int EnrollmentCount { get; set; }

        public List<CourseListItemViewModel> RelatedCourses { get; set; } = new();

        // Viewer-specific state, used to drive button text/visibility in the view
        public bool IsCurrentUserEnrolled { get; set; }
        public bool CanEnroll { get; set; } = true;
        public int ProgressPercentage { get; set; }

        /// <summary>IDs of lessons the current student has already checked off, so the view can pre-check the right boxes.</summary>
        public HashSet<int> CompletedLessonIds { get; set; } = new();

        /// <summary>True once the student has completed 100% of lessons and a certificate has been issued.</summary>
        public bool HasCertificate { get; set; }
    
}
}
