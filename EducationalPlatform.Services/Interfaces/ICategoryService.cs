using EducationalPlatform.Models.ViewModels.Category;

namespace EducationalPlatform.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(CategoryViewModel model);
        Task<(bool Success, string? Error)> UpdateAsync(CategoryViewModel model);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }
}
