namespace EducationalPlatform.Models.ViewModels.Instructor
{
    public class InstructorListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public int CourseCount { get; set; }
        public int StudentCount { get; set; }
    }

    public class InstructorDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int CourseCount { get; set; }
        public int StudentCount { get; set; }
        public List<Course.CourseListItemViewModel> Courses { get; set; } = new();
    }
}
