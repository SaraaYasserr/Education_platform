using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100), Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int CourseCount { get; set; }
    }
}
