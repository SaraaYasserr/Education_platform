using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Course
{
    public class CourseEditViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(200), Display(Name = "Course Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0, 100000), Display(Name = "Price (USD)")]
        public decimal Price { get; set; }

        public string? ExistingImageUrl { get; set; }

        [Required, Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Range(0, 10000), Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Seats Available (leave blank for unlimited)")]
        public int? SeatsAvailable { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        public List<Category.CategoryViewModel> AvailableCategories { get; set; } = new();
    }
}
