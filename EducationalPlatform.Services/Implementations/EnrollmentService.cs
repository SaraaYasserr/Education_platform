using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Enrollment;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string? Error)> EnrollAsync(string studentId, int courseId)
        {
            var course = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return (false, "Course not found.");
            }

            if (!course.IsPublished)
            {
                return (false, "This course is not currently available for enrollment.");
            }

            var existing = course.Enrollments.FirstOrDefault(e => e.StudentId == studentId);

            if (existing != null)
            {
                if (existing.Status == EnrollmentStatus.Cancelled)
                {
                    // Re-activate a previously cancelled enrollment instead of
                    // creating a duplicate row (the DB has a unique index on
                    // StudentId+CourseId).
                    existing.Status = EnrollmentStatus.Active;
                    existing.EnrollmentDate = DateTime.UtcNow;
                    _unitOfWork.EnrollmentRepository.Update(existing);
                    await _unitOfWork.SaveAsync();
                    return (true, null);
                }

                return (false, "You are already enrolled in this course.");
            }

            var activeCount = course.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled);
            if (course.SeatsAvailable.HasValue && activeCount >= course.SeatsAvailable.Value)
            {
                return (false, "Sorry, this course is full.");
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 0
            };

            await _unitOfWork.EnrollmentRepository.AddAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> CancelAsync(string studentId, int courseId)
        {
            var enrollment = (await _unitOfWork.EnrollmentRepository
                .FindAsync(e => e.StudentId == studentId && e.CourseId == courseId))
                .FirstOrDefault();

            if (enrollment == null)
            {
                return (false, "Enrollment not found.");
            }

            enrollment.Status = EnrollmentStatus.Cancelled;
            _unitOfWork.EnrollmentRepository.Update(enrollment);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<MyCoursesViewModel> GetMyCoursesAsync(string studentId)
        {
            var enrollments = await _unitOfWork.EnrollmentRepository.Query()
                .Include(e => e.Course).ThenInclude(c => c.Instructor)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            var mapped = enrollments.Select(MapToViewModel).ToList();

            return new MyCoursesViewModel
            {
                ActiveEnrollments = mapped.Where(e => e.Status == EnrollmentStatus.Active).ToList(),
                CompletedEnrollments = mapped.Where(e => e.Status == EnrollmentStatus.Completed).ToList(),
                CancelledEnrollments = mapped.Where(e => e.Status == EnrollmentStatus.Cancelled).ToList()
            };
        }

        public async Task<List<EnrollmentViewModel>> GetByCourseAsync(int courseId)
        {
            var enrollments = await _unitOfWork.EnrollmentRepository.Query()
                .Include(e => e.Student)
                .Include(e => e.Course).ThenInclude(c => c.Instructor)
                .Where(e => e.CourseId == courseId && e.Status != EnrollmentStatus.Cancelled)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            return enrollments.Select(MapToViewModel).ToList();
        }

        public async Task<bool> IsEnrolledAsync(string studentId, int courseId)
        {
            var enrollments = await _unitOfWork.EnrollmentRepository
                .FindAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Status != EnrollmentStatus.Cancelled);

            return enrollments.Any();
        }

        public async Task<(bool Success, string? Error)> UpdateProgressAsync(string studentId, int courseId, int progressPercentage)
        {
            var enrollment = (await _unitOfWork.EnrollmentRepository
                .FindAsync(e => e.StudentId == studentId && e.CourseId == courseId))
                .FirstOrDefault();

            if (enrollment == null)
            {
                return (false, "Enrollment not found.");
            }

            progressPercentage = Math.Clamp(progressPercentage, 0, 100);
            enrollment.ProgressPercentage = progressPercentage;

            if (progressPercentage >= 100)
            {
                enrollment.Status = EnrollmentStatus.Completed;
            }

            _unitOfWork.EnrollmentRepository.Update(enrollment);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        private static EnrollmentViewModel MapToViewModel(Enrollment e) => new()
        {
            Id = e.Id,
            CourseId = e.CourseId,
            CourseTitle = e.Course?.Title ?? string.Empty,
            CourseImageUrl = e.Course?.ImageUrl,
            CoursePrice = e.Course?.Price ?? 0,
            InstructorName = e.Course?.Instructor?.FullName ?? string.Empty,
            StudentId = e.StudentId,
            StudentName = e.Student?.FullName ?? string.Empty,
            StudentEmail = e.Student?.Email,
            EnrollmentDate = e.EnrollmentDate,
            Status = e.Status,
            ProgressPercentage = e.ProgressPercentage
        };
    }
}
