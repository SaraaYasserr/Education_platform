using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Category;
using EducationalPlatform.Models.ViewModels.Course;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CourseController(
            ICourseService courseService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _userManager = userManager;
            _environment = environment;
        }

        // GET /Course - matches course.html (the all-courses grid with search/filter)
        [HttpGet]
        public async Task<IActionResult> Index(CourseListViewModel filter)
        {
            var result = await _courseService.SearchAsync(filter);
            return View(result);
        }

        // GET /Course/Details/5 - matches course_details.html
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var course = await _courseService.GetDetailsAsync(id, currentUserId);

            if (course == null) return NotFound();

            return View(course);
        }

        // ===== Instructor / Admin course management =====

        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CourseCreateViewModel
            {
                AvailableCategories = await _categoryService.GetAllAsync()
            };

            return View(model);
        }

        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await _categoryService.GetAllAsync();
                return View(model);
            }

            var instructorId = _userManager.GetUserId(User)!;

            var (success, error, courseId) = await _courseService.CreateAsync(model, imageFile, instructorId, _environment.WebRootPath);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Could not create the course.");
                model.AvailableCategories = await _categoryService.GetAllAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = "Course created successfully.";
            return RedirectToAction("Details", new { id = courseId });
        }

        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!await _courseService.IsOwnerOrAdminAsync(id, userId, isAdmin))
            {
                return Forbid();
            }

            var model = await _courseService.GetForEditAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseEditViewModel model, IFormFile? imageFile)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await _categoryService.GetAllAsync();
                return View(model);
            }

            var (success, error) = await _courseService.UpdateAsync(model, imageFile, userId, isAdmin, _environment.WebRootPath);

            if (!success)
            {
                if (error == "You are not allowed to edit this course.")
                {
                    return Forbid();
                }

                ModelState.AddModelError(string.Empty, error ?? "Could not update the course.");
                model.AvailableCategories = await _categoryService.GetAllAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = "Course updated successfully.";
            return RedirectToAction("Details", new { id = model.Id });
        }

        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            var (success, error) = await _courseService.DeleteAsync(id, userId, isAdmin);

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Could not delete the course.";
                return RedirectToAction("Details", new { id });
            }

            TempData["SuccessMessage"] = "Course deleted.";
            return RedirectToAction(isAdmin ? "Admin" : "Instructor", "Dashboard");
        }

        // Instructor's own courses ("Manage Own Courses")
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [HttpGet]
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User)!;
            var courses = await _courseService.GetByInstructorAsync(userId);
            return View(courses);
        }
    }
}
