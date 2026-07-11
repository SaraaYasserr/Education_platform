using EducationalPlatform.Models.Entities;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class LessonProgressService : ILessonProgressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LessonProgressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HashSet<int>> GetCompletedLessonIdsAsync(string studentId, int courseId)
        {
            var records = await _unitOfWork.LessonProgressRepository.FindAsync(
                p => p.StudentId == studentId && p.CourseId == courseId && p.IsCompleted);

            return records.Select(p => p.LessonId).ToHashSet();
        }

        public async Task<int> GetProgressPercentageAsync(string studentId, int courseId)
        {
            var totalLessons = (await _unitOfWork.LessonRepository.FindAsync(l => l.CourseId == courseId)).Count();
            if (totalLessons == 0) return 0;

            var completedCount = (await _unitOfWork.LessonProgressRepository.FindAsync(
                p => p.StudentId == studentId && p.CourseId == courseId && p.IsCompleted)).Count();

            return (int)Math.Round(completedCount * 100.0 / totalLessons);
        }

        public async Task<(bool Success, string? Error, int ProgressPercentage, bool CertificateIssued)> ToggleLessonCompletionAsync(
            string studentId, int lessonId, bool isCompleted)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                return (false, "Lesson not found.", 0, false);
            }

            var isEnrolled = (await _unitOfWork.EnrollmentRepository.FindAsync(
                e => e.StudentId == studentId && e.CourseId == lesson.CourseId && e.Status != EnrollmentStatus.Cancelled)).Any();

            if (!isEnrolled)
            {
                return (false, "You must be enrolled in this course to track lesson progress.", 0, false);
            }

            var existing = await _unitOfWork.LessonProgressRepository
                .Query()
                .FirstOrDefaultAsync(p =>
                    p.StudentId == studentId &&
                    p.LessonId == lessonId);

            if (existing == null)
            {
                existing = new LessonProgress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    CourseId = lesson.CourseId,
                    IsCompleted = isCompleted,
                    CompletedAt = isCompleted ? DateTime.UtcNow : null
                };
                await _unitOfWork.LessonProgressRepository.AddAsync(existing);
            }
            else
            {
                existing.IsCompleted = isCompleted;
                existing.CompletedAt = isCompleted ? DateTime.UtcNow : null;
                _unitOfWork.LessonProgressRepository.Update(existing);
            }

            await _unitOfWork.SaveAsync();

            var progressPercentage = await GetProgressPercentageAsync(studentId, lesson.CourseId);

            var enrollment = (await _unitOfWork.EnrollmentRepository.FindAsync(
                e => e.StudentId == studentId && e.CourseId == lesson.CourseId)).FirstOrDefault();

            var certificateIssued = false;

            if (enrollment != null)
            {
                enrollment.ProgressPercentage = progressPercentage;
                enrollment.Status = progressPercentage >= 100 ? EnrollmentStatus.Completed : EnrollmentStatus.Active;
                _unitOfWork.EnrollmentRepository.Update(enrollment);
                await _unitOfWork.SaveAsync();

                if (progressPercentage >= 100)
                {
                    var alreadyHasCertificate = (await _unitOfWork.CertificateRepository.FindAsync(
                        c => c.StudentId == studentId && c.CourseId == lesson.CourseId)).Any();

                    if (!alreadyHasCertificate)
                    {
                        var certificate = new Certificate
                        {
                            StudentId = studentId,
                            CourseId = lesson.CourseId,
                            CertificateCode = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant(),
                            IssuedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.CertificateRepository.AddAsync(certificate);
                        await _unitOfWork.SaveAsync();
                        certificateIssued = true;
                    }
                }
            }

            return (true, null, progressPercentage, certificateIssued);
        }
    }
}