using EducationalPlatform.Models.Entities;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            IDashboardService dashboardService,
            IUserService userService,
            IContactService contactService,
            UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userService = userService;
            _contactService = contactService;
            _userManager = userManager;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            var model = await _dashboardService.GetAdminDashboardAsync();
            return View(model);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpGet]
        public async Task<IActionResult> Instructor()
        {
            var instructorId = _userManager.GetUserId(User)!;
            var model = await _dashboardService.GetInstructorDashboardAsync(instructorId);
            return View(model);
        }

        [Authorize(Roles = Roles.Student)]
        [HttpGet]
        public async Task<IActionResult> Student()
        {
            var studentId = _userManager.GetUserId(User)!;
            var model = await _dashboardService.GetStudentDashboardAsync(studentId);
            return View(model);
        }

        // ===== Admin: manage users =====
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (success, error) = await _userService.DeleteUserAsync(id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "User deleted." : error;

            return RedirectToAction("Users");
        }

        // ===== Admin: view contact messages =====
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Messages()
        {
            var messages = await _contactService.GetAllAsync();
            return View(messages);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkMessageRead(int id)
        {
            await _contactService.MarkAsReadAsync(id);
            return RedirectToAction("Messages");
        }
    }
}
