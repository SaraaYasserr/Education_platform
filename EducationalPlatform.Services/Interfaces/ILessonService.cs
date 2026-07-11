using EducationalPlatform.Models.ViewModels.Lesson;

namespace EducationalPlatform.Services.Interfaces
{
    public interface ILessonService
    {
        Task<List<LessonViewModel>> GetByCourseAsync(int courseId);
        Task<LessonViewModel?> GetByIdAsync(int id);
        Task<LessonEditViewModel?> GetForEditAsync(int id);

        Task<(bool Success, string? Error)> CreateAsync(LessonCreateViewModel model, string userId, bool isAdmin);
        Task<(bool Success, string? Error)> UpdateAsync(LessonEditViewModel model, string userId, bool isAdmin);
        Task<(bool Success, string? Error)> DeleteAsync(int id, string userId, bool isAdmin);
    }
}
