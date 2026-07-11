using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Account
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required, MaxLength(150), Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? ProfileImage { get; set; }

        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
