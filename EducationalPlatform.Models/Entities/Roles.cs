namespace EducationalPlatform.Models.Entities
{
    /// <summary>
    /// Centralized role name constants so "Admin"/"Instructor"/"Student" are
    /// never hard-coded as magic strings scattered across the project.
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Instructor = "Instructor";
        public const string Student = "Student";

        public static readonly string[] All = { Admin, Instructor, Student };
    }
}
