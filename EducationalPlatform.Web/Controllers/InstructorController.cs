using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IUserService _userService;

        public InstructorController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var instructors = await _userService.GetAllInstructorsAsync();
            return View(instructors);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var instructor = await _userService.GetInstructorDetailsAsync(id);
            if (instructor == null) return NotFound();

            return View(instructor);
        }
    }
}
