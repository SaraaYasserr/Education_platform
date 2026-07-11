using EducationalPlatform.Models.ViewModels.Shared;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IContactService _contactService;

        public HomeController(ICourseService courseService, IContactService contactService)
        {
            _courseService = courseService;
            _contactService = contactService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredCourses = await _courseService.GetFeaturedAsync(6);
            return View(featuredCourses);
        }

        public IActionResult About() => View();

        public IActionResult Faq() => View();

        [HttpGet]
        public IActionResult Contact() => View(new ContactViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _contactService.SubmitAsync(model);

            TempData["SuccessMessage"] = "Thank you! Your message has been sent. We'll get back to you soon.";
            return RedirectToAction("Contact");
        }

        public IActionResult StatusCode(int id)
        {
            var model = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                StatusCode = id,
                Message = id switch
                {
                    404 => "The page you're looking for could not be found.",
                    403 => "You don't have permission to access this page.",
                    _ => "Something went wrong while processing your request."
                }
            };

            Response.StatusCode = id;
            return View("Error", model);
        }

        public IActionResult Error()
        {
            var model = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                StatusCode = 500,
                Message = "An unexpected error occurred."
            };

            return View(model);
        }
    }
}
