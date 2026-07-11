using EducationalPlatform.Models.ViewModels.Course;
using Microsoft.AspNetCore.Http;

namespace EducationalPlatform.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseListViewModel> SearchAsync(CourseListViewModel filter);

        Task<CourseDetailsViewModel?> GetDetailsAsync(int courseId, string? currentUserId);

        Task<CourseEditViewModel?> GetForEditAsync(int courseId);

        /// <summary>Courses taught by a specific instructor (Instructor "Manage Own Courses").</summary>
        Task<List<CourseListItemViewModel>> GetByInstructorAsync(string instructorId);

        Task<(bool Success, string? Error, int CourseId)> CreateAsync(CourseCreateViewModel model, IFormFile? imageFile, string instructorId, string webRootPath);

        Task<(bool Success, string? Error)> UpdateAsync(CourseEditViewModel model, IFormFile? imageFile, string instructorId, bool isAdmin, string webRootPath);

        Task<(bool Success, string? Error)> DeleteAsync(int courseId, string instructorId, bool isAdmin);

        /// <summary>True if the given instructor owns the course (or the caller is an Admin).</summary>
        Task<bool> IsOwnerOrAdminAsync(int courseId, string userId, bool isAdmin);

        Task<List<CourseListItemViewModel>> GetFeaturedAsync(int count = 6);
        Task<CourseDetailsViewModel?> GetCourseForPaymentAsync(int courseId);
    }
}
