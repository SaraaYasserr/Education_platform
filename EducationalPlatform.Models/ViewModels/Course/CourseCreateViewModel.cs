using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Course
{
    public class CourseCreateViewModel
    {
        [Required, MaxLength(200), Display(Name = "Course Title")]
        public string Title { get; set; } = string.Empty;

        [Required, Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0, 100000), Display(Name = "Price (USD)")]
        public decimal Price { get; set; }

        [Required, Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Range(0, 10000), Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Seats Available (leave blank for unlimited)")]
        public int? SeatsAvailable { get; set; }

        [Display(Name = "Publish immediately")]
        public bool IsPublished { get; set; } = true;

        /// <summary>Populated for the Category dropdown in the view.</summary>
        public List<Category.CategoryViewModel> AvailableCategories { get; set; } = new();
    }
}
