using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Review
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? StudentImage { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Backs the "Add a Review" form on course_details.html. Name/Email are
    /// no longer free-text inputs in our version - the student is identified
    /// from their logged-in account, so only Rating + Comment are submitted.
    /// </summary>
    public class ReviewCreateViewModel
    {
        public int CourseId { get; set; }

        [Required, Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars.")]
        [Display(Name = "Your Rating")]
        public int Rating { get; set; }

        [MaxLength(1000), Display(Name = "Your Review")]
        public string? Comment { get; set; }
    }
}
