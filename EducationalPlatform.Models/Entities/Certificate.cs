using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.Entities
{
    /// <summary>
    /// Issued automatically the moment a student completes 100% of a
    /// course's lessons. One certificate per (StudentId, CourseId) - see
    /// the unique index in CertificateConfiguration.
    /// </summary>
    public class Certificate
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public int CourseId { get; set; }

        /// <summary>Short human-shareable code (e.g. "A1B2C3D4E5F6") used for public verification.</summary>
        [Required, MaxLength(20)]
        public string CertificateCode { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
