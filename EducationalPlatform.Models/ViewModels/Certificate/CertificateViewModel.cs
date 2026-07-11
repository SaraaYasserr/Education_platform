namespace EducationalPlatform.Models.ViewModels.Certificate
{
    public class CertificateViewModel
    {
        public int Id { get; set; }
        public string CertificateCode { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }

        public string StudentName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; }
    }
}
