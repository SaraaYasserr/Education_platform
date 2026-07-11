using Microsoft.AspNetCore.Identity;

namespace EducationalPlatform.Models.Entities
{
    /// <summary>
    /// Extends the built-in ASP.NET Core Identity user with the extra profile
    /// fields the platform needs (name, photo, etc.). Roles are NOT a property
    /// here - they are managed separately through Identity's role tables
    /// (AspNetRoles / AspNetUserRoles) and assigned via UserManager.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties

        /// <summary>Courses this user teaches, when the user is in the Instructor role.</summary>
        public ICollection<Course> CoursesTaught { get; set; } = new List<Course>();

        /// <summary>Courses this user is enrolled in, when the user is in the Student role.</summary>
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
        public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

        /// <summary>Certificates this student has earned by completing courses.</summary>
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}
