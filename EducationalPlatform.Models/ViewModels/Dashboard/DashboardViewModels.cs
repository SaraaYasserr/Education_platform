namespace EducationalPlatform.Models.ViewModels.Dashboard
{
    /// <summary>Admin dashboard: site-wide statistics + management shortcuts.</summary>
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalCategories { get; set; }
        public int TotalEnrollments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UnreadContactMessages { get; set; }

        public List<RecentEnrollmentItem> RecentEnrollments { get; set; } = new();
        public List<TopCourseItem> TopCourses { get; set; } = new();
    }

    public class RecentEnrollmentItem
    {
        public string StudentName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
    }

    public class TopCourseItem
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
        public double AverageRating { get; set; }
    }

    /// <summary>Instructor dashboard: their own courses + enrolled students.</summary>
    public class InstructorDashboardViewModel
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalLessons { get; set; }
        public double AverageRating { get; set; }

        public List<TopCourseItem> MyCourses { get; set; } = new();
        public List<RecentEnrollmentItem> RecentStudents { get; set; } = new();
    }

    /// <summary>Student dashboard: enrolled courses + progress.</summary>
    public class StudentDashboardViewModel
    {
        public int EnrolledCourseCount { get; set; }
        public int CompletedCourseCount { get; set; }
        public double AverageProgress { get; set; }

        public List<Enrollment.EnrollmentViewModel> MyCourses { get; set; } = new();
    }
}
