using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Category;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;

namespace EducationalPlatform.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryViewModel>> GetAllAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllWithIncludeAsync(c => c.Courses);

            return categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CourseCount = c.Courses.Count
                })
                .ToList();
        }

        public async Task<CategoryViewModel?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetWithIncludeAsync(c => c.Id == id, c => c.Courses);

            if (category == null) return null;

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CourseCount = category.Courses.Count
            };
        }

        public async Task<(bool Success, string? Error)> CreateAsync(CategoryViewModel model)
        {
            var existing = await _unitOfWork.CategoryRepository.FindAsync(c => c.Name == model.Name);
            if (existing.Any())
            {
                return (false, "A category with this name already exists.");
            }

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(CategoryViewModel model)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(model.Id);
            if (category == null)
            {
                return (false, "Category not found.");
            }

            var duplicate = (await _unitOfWork.CategoryRepository.FindAsync(c => c.Name == model.Name && c.Id != model.Id)).Any();
            if (duplicate)
            {
                return (false, "Another category already uses this name.");
            }

            category.Name = model.Name;
            category.Description = model.Description;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetWithIncludeAsync(c => c.Id == id, c => c.Courses);
            if (category == null)
            {
                return (false, "Category not found.");
            }

            if (category.Courses.Any())
            {
                return (false, "This category has courses assigned to it. Reassign or delete those courses first.");
            }

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }
    }
}
