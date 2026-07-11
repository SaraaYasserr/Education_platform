using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.Entities
{
    /// <summary>
    /// Stores submissions from the public Contact Us form (replaces the
    /// original template's contact.php "mail()" call). An Admin can review
    /// these from the Dashboard instead of relying on an SMTP server, which
    /// keeps the project runnable without extra email configuration.
    /// </summary>
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
