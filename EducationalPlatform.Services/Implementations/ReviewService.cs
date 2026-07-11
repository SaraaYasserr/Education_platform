using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Review;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ReviewViewModel>> GetByCourseAsync(int courseId)
        {
            var reviews = await _unitOfWork.ReviewRepository.Query()
                .Include(r => r.Student)
                .Where(r => r.CourseId == courseId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                StudentName = r.Student.FullName,
                StudentImage = r.Student.ProfileImage,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<(bool Success, string? Error)> CreateAsync(ReviewCreateViewModel model, string studentId)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(model.CourseId);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            var isEnrolled = (await _unitOfWork.EnrollmentRepository
                .FindAsync(e => e.StudentId == studentId && e.CourseId == model.CourseId && e.Status != EnrollmentStatus.Cancelled))
                .Any();

            if (!isEnrolled)
            {
                return (false, "Only enrolled students can review this course.");
            }

            var alreadyReviewed = (await _unitOfWork.ReviewRepository
                .FindAsync(r => r.StudentId == studentId && r.CourseId == model.CourseId))
                .Any();

            if (alreadyReviewed)
            {
                return (false, "You have already reviewed this course.");
            }

            var review = new Review
            {
                StudentId = studentId,
                CourseId = model.CourseId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int reviewId, string userId, bool isAdmin)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                return (false, "Review not found.");
            }

            if (!isAdmin && review.StudentId != userId)
            {
                return (false, "You are not allowed to delete this review.");
            }

            _unitOfWork.ReviewRepository.Delete(review);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<bool> HasUserReviewedAsync(string studentId, int courseId)
        {
            var reviews = await _unitOfWork.ReviewRepository
                .FindAsync(r => r.StudentId == studentId && r.CourseId == courseId);

            return reviews.Any();
        }
    }
}
