using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Lesson;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LessonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LessonViewModel>> GetByCourseAsync(int courseId)
        {
            var lessons = await _unitOfWork.LessonRepository.Query()
                .Include(l => l.Course)
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.LessonOrder)
                .ToListAsync();

            return lessons.Select(MapToViewModel).ToList();
        }

        public async Task<LessonViewModel?> GetByIdAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository.GetWithIncludeAsync(l => l.Id == id, l => l.Course);
            return lesson == null ? null : MapToViewModel(lesson);
        }

        public async Task<LessonEditViewModel?> GetForEditAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository.GetWithIncludeAsync(l => l.Id == id, l => l.Course);
            if (lesson == null) return null;

            return new LessonEditViewModel
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                CourseTitle = lesson.Course.Title,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                LessonOrder = lesson.LessonOrder,
                DurationMinutes = lesson.DurationMinutes
            };
        }

        public async Task<(bool Success, string? Error)> CreateAsync(LessonCreateViewModel model, string userId, bool isAdmin)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(model.CourseId);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            if (!isAdmin && course.InstructorId != userId)
            {
                return (false, "You are not allowed to add lessons to this course.");
            }

            var lesson = new Lesson
            {
                CourseId = model.CourseId,
                Title = model.Title,
                Content = model.Content,
                VideoUrl = model.VideoUrl,
                LessonOrder = model.LessonOrder,
                DurationMinutes = model.DurationMinutes
            };

            await _unitOfWork.LessonRepository.AddAsync(lesson);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(LessonEditViewModel model, string userId, bool isAdmin)
        {
            var lesson = await _unitOfWork.LessonRepository.GetWithIncludeAsync(l => l.Id == model.Id, l => l.Course);
            if (lesson == null)
            {
                return (false, "Lesson not found.");
            }

            if (!isAdmin && lesson.Course.InstructorId != userId)
            {
                return (false, "You are not allowed to edit this lesson.");
            }

            lesson.Title = model.Title;
            lesson.Content = model.Content;
            lesson.VideoUrl = model.VideoUrl;
            lesson.LessonOrder = model.LessonOrder;
            lesson.DurationMinutes = model.DurationMinutes;

            _unitOfWork.LessonRepository.Update(lesson);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var lesson = await _unitOfWork.LessonRepository.GetWithIncludeAsync(l => l.Id == id, l => l.Course);
            if (lesson == null)
            {
                return (false, "Lesson not found.");
            }

            if (!isAdmin && lesson.Course.InstructorId != userId)
            {
                return (false, "You are not allowed to delete this lesson.");
            }

            _unitOfWork.LessonRepository.Delete(lesson);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        private static LessonViewModel MapToViewModel(Lesson l) => new()
        {
            Id = l.Id,
            Title = l.Title,
            Content = l.Content,
            VideoUrl = l.VideoUrl,
            LessonOrder = l.LessonOrder,
            DurationMinutes = l.DurationMinutes,
            CourseId = l.CourseId,
            CourseTitle = l.Course?.Title ?? string.Empty
        };
    }
}
