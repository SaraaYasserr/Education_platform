using System.ComponentModel.DataAnnotations;
using EducationalPlatform.Models.Entities;

namespace EducationalPlatform.Models.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
    }

    public class EnrollmentCreateDto
    {
        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }
    }
}
