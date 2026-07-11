using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Lesson;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
    public class LessonController : Controller
    {
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LessonController(ILessonService lessonService, ICourseService courseService, UserManager<ApplicationUser> userManager)
        {
            _lessonService = lessonService;
            _courseService = courseService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int courseId)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!await _courseService.IsOwnerOrAdminAsync(courseId, userId, isAdmin))
            {
                return Forbid();
            }

            var lessons = await _lessonService.GetByCourseAsync(courseId);
            ViewBag.CourseId = courseId;
            return View(lessons);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int courseId)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!await _courseService.IsOwnerOrAdminAsync(courseId, userId, isAdmin))
            {
                return Forbid();
            }

            var course = await _courseService.GetForEditAsync(courseId);
            if (course == null) return NotFound();

            return View(new LessonCreateViewModel { CourseId = courseId, CourseTitle = course.Title });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            var (success, error) = await _lessonService.CreateAsync(model, userId, isAdmin);

            if (!success)
            {
                if (error == "You are not allowed to add lessons to this course.") return Forbid();

                ModelState.AddModelError(string.Empty, error ?? "Could not create the lesson.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Lesson added successfully.";
            return RedirectToAction("Index", new { courseId = model.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _lessonService.GetForEditAsync(id);
            if (model == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!await _courseService.IsOwnerOrAdminAsync(model.CourseId, userId, isAdmin))
            {
                return Forbid();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LessonEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            var (success, error) = await _lessonService.UpdateAsync(model, userId, isAdmin);

            if (!success)
            {
                if (error == "You are not allowed to edit this lesson.") return Forbid();

                ModelState.AddModelError(string.Empty, error ?? "Could not update the lesson.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Lesson updated successfully.";
            return RedirectToAction("Index", new { courseId = model.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int courseId)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            var (success, error) = await _lessonService.DeleteAsync(id, userId, isAdmin);

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Could not delete the lesson.";
            }
            else
            {
                TempData["SuccessMessage"] = "Lesson deleted.";
            }

            return RedirectToAction("Index", new { courseId });
        }
    }
}
