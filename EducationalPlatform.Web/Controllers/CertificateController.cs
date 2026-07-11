using EducationalPlatform.Models.Entities;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    public class CertificateController : Controller
    {
        private readonly ICertificateService _certificateService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CertificateController(ICertificateService certificateService, UserManager<ApplicationUser> userManager)
        {
            _certificateService = certificateService;
            _userManager = userManager;
        }

        // The student's own certificate for one course - the "View Certificate" button on Course Details links here.
        [Authorize(Roles = Roles.Student)]
        [HttpGet]
        public async Task<IActionResult> View(int courseId)
        {
            var studentId = _userManager.GetUserId(User)!;
            var certificate = await _certificateService.GetByCourseAsync(studentId, courseId);

            if (certificate == null) return NotFound();

            return View(certificate);
        }

        // All certificates the logged-in student has earned.
        [Authorize(Roles = Roles.Student)]
        [HttpGet]
        public async Task<IActionResult> MyCertificates()
        {
            var studentId = _userManager.GetUserId(User)!;
            var certificates = await _certificateService.GetMyCertificatesAsync(studentId);
            return View(certificates);
        }

        // Public page: anyone with the certificate code (e.g. an employer) can verify it's genuine.
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Verify(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return View((EducationalPlatform.Models.ViewModels.Certificate.CertificateViewModel?)null);

            var certificate = await _certificateService.GetByCodeAsync(code);
            return View(certificate);
        }
    }
}
