using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required, MaxLength(150), Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// The role the user registers as. Only "Student" or "Instructor" may
        /// be self-selected here; "Admin" accounts are never created through
        /// public registration (they are seeded or promoted by another Admin).
        /// </summary>
        [Required, Display(Name = "I am a")]
        public string Role { get; set; } = Entities.Roles.Student;
    }
}
