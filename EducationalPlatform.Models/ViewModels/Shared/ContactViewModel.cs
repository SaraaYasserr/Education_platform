using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Shared
{
    /// <summary>Backs contact.html's form (replaces contact.php).</summary>
    public class ContactViewModel
    {
        [Required, MaxLength(150), Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Your Email")]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(200), Display(Name = "Your Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required, MaxLength(2000), Display(Name = "Your Message")]
        public string Message { get; set; } = string.Empty;
    }

    public class ContactMessageListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
