using EducationalPlatform.Models.ViewModels.Payment;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EducationalPlatform.Models.Entities;

namespace EducationalPlatform.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IMembershipService _membershipService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(
            ICourseService courseService,
            IMembershipService membershipService,
            IEnrollmentService enrollmentService,
        UserManager<ApplicationUser> userManager)
        {
            _courseService = courseService;
            _membershipService = membershipService;
            _enrollmentService = enrollmentService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int? courseId, int? planId)
        {
            var model = new PaymentViewModel();

            if (courseId.HasValue)
            {
                var course = await _courseService.GetCourseForPaymentAsync(courseId.Value);

                if (course == null)
                    return NotFound();

                model.CourseId = course.Id;
                model.ItemName = course.Title;
                model.Amount = course.Price;
                model.IsSubscription = false;
            }
            else if (planId.HasValue)
            {
                var plan = await _membershipService.GetPlanAsync(planId.Value);

                if (plan == null)
                    return NotFound();

                model.PlanId = plan.Id;
                model.ItemName = plan.Name;
                model.Amount = plan.Price;
                model.IsSubscription = true;
            }
            else
            {
                return BadRequest();
            }

            return View(model);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}");

                    foreach (var e in error.Value.Errors)
                    {
                        Console.WriteLine(e.ErrorMessage);
                    }
                }

                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;

            //payment Success

            if (model.IsSubscription)
            {
                var (success, error) =
                    await _membershipService.SubscribeAsync(userId, model.PlanId!.Value);

                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success ? "Payment Successful. Subscription Activated!"
                            : error;

                return RedirectToAction("Pricing", "Membership");
            }

            var result =
                await _enrollmentService.EnrollAsync(userId, model.CourseId!.Value);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? "Payment Successful. Enrollment Completed!"
                               : result.Error;

            return RedirectToAction("Details", "Course",
                new { id = model.CourseId });
        }
    }
}