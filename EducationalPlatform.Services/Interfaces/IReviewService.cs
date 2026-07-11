using EducationalPlatform.Models.ViewModels.Review;

namespace EducationalPlatform.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewViewModel>> GetByCourseAsync(int courseId);
        Task<(bool Success, string? Error)> CreateAsync(ReviewCreateViewModel model, string studentId);
        Task<(bool Success, string? Error)> DeleteAsync(int reviewId, string userId, bool isAdmin);
        Task<bool> HasUserReviewedAsync(string studentId, int courseId);
    }
}
