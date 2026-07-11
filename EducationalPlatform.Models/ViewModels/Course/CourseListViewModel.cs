namespace EducationalPlatform.Models.ViewModels.Course
{
    /// <summary>Lightweight shape used for course grid/cards on the listing page.</summary>
    public class CourseListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int LessonCount { get; set; }
        public int DurationMinutes { get; set; }
        public int EnrollmentCount { get; set; }
    }

    public class CourseListViewModel
    {
        public List<CourseListItemViewModel> Courses { get; set; } = new();
        public List<Category.CategoryViewModel> Categories { get; set; } = new();

        // Filter / search state, bound back from the query string
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } // "newest", "price_asc", "price_desc", "rating"

        // Paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalCount { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
