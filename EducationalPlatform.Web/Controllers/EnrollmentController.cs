using EducationalPlatform.Models.Entities;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;
        private readonly ILessonProgressService _lessonProgressService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentController(
              IEnrollmentService enrollmentService,
              ICourseService courseService,
              ILessonProgressService lessonProgressService,
              UserManager<ApplicationUser> userManager)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _lessonProgressService = lessonProgressService;
            _userManager = userManager;
        }

        [Authorize(Roles = Roles.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var studentId = _userManager.GetUserId(User)!;

            var (success, error) = await _enrollmentService.EnrollAsync(studentId, courseId);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "You have successfully enrolled in this course!" : error;

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        [Authorize(Roles = Roles.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int courseId)
        {
            var studentId = _userManager.GetUserId(User)!;

            var (success, error) = await _enrollmentService.CancelAsync(studentId, courseId);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Your enrollment has been cancelled." : error;

            return RedirectToAction("MyCourses");
        }

        [Authorize(Roles = Roles.Student)]
        [HttpGet]
        public async Task<IActionResult> MyCourses()
        {
            var studentId = _userManager.GetUserId(User)!;
            var model = await _enrollmentService.GetMyCoursesAsync(studentId);
            return View(model);
        }

        // Instructor / Admin: view students enrolled in a specific course.
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [HttpGet]
        public async Task<IActionResult> CourseStudents(int courseId)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!await _courseService.IsOwnerOrAdminAsync(courseId, userId, isAdmin))
            {
                return Forbid();
            }

            var students = await _enrollmentService.GetByCourseAsync(courseId);
            ViewBag.CourseId = courseId;
            return View(students);
        }

        [Authorize(Roles = Roles.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int courseId, int progress)
        {
            var studentId = _userManager.GetUserId(User)!;
            await _enrollmentService.UpdateProgressAsync(studentId, courseId, progress);
            return RedirectToAction("MyCourses");
        }
        [Authorize(Roles = Roles.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLessonComplete(int lessonId, int courseId, bool isCompleted)
        {
            var studentId = _userManager.GetUserId(User)!;

            var (success, error, _, certificateIssued) =
                await _lessonProgressService.ToggleLessonCompletionAsync(studentId, lessonId, isCompleted);

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Could not update lesson progress.";
            }
            else if (certificateIssued)
            {
                TempData["SuccessMessage"] = "?? Congratulations! You've completed the course and earned a certificate!";
            }

            return RedirectToAction("Details", "Course", new { id = courseId });
        }
    }
}