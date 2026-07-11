namespace EducationalPlatform.Models.Entities
{
    public enum EnrollmentStatus
    {
        Active = 0,
        Cancelled = 1,
        Completed = 2
    }

    public class Enrollment
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        /// <summary>Simple 0-100 progress indicator the student/instructor can see.</summary>
        public int ProgressPercentage { get; set; } = 0;

        // Navigation properties
        public ApplicationUser Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
