using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Review;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        // Submitted from the "Add a Review" form on course_details.html.
        [Authorize(Roles = Roles.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please select a rating before submitting your review.";
                return RedirectToAction("Details", "Course", new { id = model.CourseId });
            }

            var studentId = _userManager.GetUserId(User)!;

            var (success, error) = await _reviewService.CreateAsync(model, studentId);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Thank you! Your review has been posted." : error;

            return RedirectToAction("Details", "Course", new { id = model.CourseId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int courseId)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole(Roles.Admin);

            var (success, error) = await _reviewService.DeleteAsync(id, userId, isAdmin);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Review deleted." : error;

            return RedirectToAction("Details", "Course", new { id = courseId });
        }
    }
}
