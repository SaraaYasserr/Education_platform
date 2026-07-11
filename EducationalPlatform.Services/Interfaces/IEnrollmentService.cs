using EducationalPlatform.Models.ViewModels.Enrollment;

namespace EducationalPlatform.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<(bool Success, string? Error)> EnrollAsync(string studentId, int courseId);
        Task<(bool Success, string? Error)> CancelAsync(string studentId, int courseId);

        Task<MyCoursesViewModel> GetMyCoursesAsync(string studentId);

        /// <summary>For the Instructor "View Enrolled Students" feature.</summary>
        Task<List<EnrollmentViewModel>> GetByCourseAsync(int courseId);

        Task<bool> IsEnrolledAsync(string studentId, int courseId);

        Task<(bool Success, string? Error)> UpdateProgressAsync(string studentId, int courseId, int progressPercentage);
    }
}
