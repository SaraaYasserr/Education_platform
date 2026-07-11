using EducationalPlatform.Models.ViewModels.Certificate;

namespace EducationalPlatform.Services.Interfaces
{
    public interface ICertificateService
    {
        /// <summary>The certificate a student earned for a specific course, or null if not yet earned.</summary>
        Task<CertificateViewModel?> GetByCourseAsync(string studentId, int courseId);

        /// <summary>Public lookup by the certificate's shareable code, for a "Verify Certificate" page.</summary>
        Task<CertificateViewModel?> GetByCodeAsync(string certificateCode);

        /// <summary>All certificates a student has earned, most recent first.</summary>
        Task<List<CertificateViewModel>> GetMyCertificatesAsync(string studentId);
    }
}
