using EducationalPlatform.Models.Entities;

namespace EducationalPlatform.Models.ViewModels.Enrollment
{
    public class EnrollmentViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string? CourseImageUrl { get; set; }
        public decimal CoursePrice { get; set; }
        public string InstructorName { get; set; } = string.Empty;

        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? StudentEmail { get; set; }

        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
    }

    /// <summary>Container for "My Courses" page (student view).</summary>
    public class MyCoursesViewModel
    {
        public List<EnrollmentViewModel> ActiveEnrollments { get; set; } = new();
        public List<EnrollmentViewModel> CompletedEnrollments { get; set; } = new();
        public List<EnrollmentViewModel> CancelledEnrollments { get; set; } = new();
    }
}
