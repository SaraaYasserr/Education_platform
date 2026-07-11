using EducationalPlatform.Models.ViewModels.Certificate;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class CertificateService : ICertificateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CertificateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CertificateViewModel?> GetByCourseAsync(string studentId, int courseId)
        {
            var certificate = await _unitOfWork.CertificateRepository.Query()
                .Include(c => c.Student)
                .Include(c => c.Course).ThenInclude(course => course.Instructor)
                .FirstOrDefaultAsync(c => c.StudentId == studentId && c.CourseId == courseId);

            return certificate == null ? null : MapToViewModel(certificate);
        }

        public async Task<CertificateViewModel?> GetByCodeAsync(string certificateCode)
        {
            var certificate = await _unitOfWork.CertificateRepository.Query()
                .Include(c => c.Student)
                .Include(c => c.Course).ThenInclude(course => course.Instructor)
                .FirstOrDefaultAsync(c => c.CertificateCode == certificateCode);

            return certificate == null ? null : MapToViewModel(certificate);
        }

        public async Task<List<CertificateViewModel>> GetMyCertificatesAsync(string studentId)
        {
            var certificates = await _unitOfWork.CertificateRepository.Query()
                .Include(c => c.Student)
                .Include(c => c.Course).ThenInclude(course => course.Instructor)
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.IssuedAt)
                .ToListAsync();

            return certificates.Select(MapToViewModel).ToList();
        }

        private static CertificateViewModel MapToViewModel(Models.Entities.Certificate c) => new()
        {
            Id = c.Id,
            CertificateCode = c.CertificateCode,
            CourseId = c.CourseId,
            CourseTitle = c.Course.Title,
            DurationMinutes = c.Course.DurationMinutes,
            StudentName = c.Student.FullName,
            InstructorName = c.Course.Instructor.FullName,
            IssuedAt = c.IssuedAt
        };
    }
}
