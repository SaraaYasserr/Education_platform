using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Membership;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MembershipController(IMembershipService membershipService, UserManager<ApplicationUser> userManager)
        {
            _membershipService = membershipService;
            _userManager = userManager;
        }

        // GET /Membership/Pricing - matches pricing.html
        [HttpGet]
        public async Task<IActionResult> Pricing()
        {
            var currentUserId = _userManager.GetUserId(User);
            var model = await _membershipService.GetPricingPageAsync(currentUserId);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(int planId)
        {
            var userId = _userManager.GetUserId(User)!;

            var (success, error) = await _membershipService.SubscribeAsync(userId, planId);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "You're subscribed! Enjoy your new plan." : error;

            return RedirectToAction("Pricing");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MySubscription()
        {
            var userId = _userManager.GetUserId(User)!;
            var model = await _membershipService.GetMySubscriptionAsync(userId);
            return View(model);
        }

        // ===== Admin: manage subscription plans =====

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> ManagePlans()
        {
            var plans = await _membershipService.GetAllPlansAsync();
            return View(plans);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public IActionResult CreatePlan() => View(new SubscriptionPlanEditViewModel());

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlan(SubscriptionPlanEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _membershipService.CreatePlanAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Could not create the plan.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Plan created successfully.";
            return RedirectToAction("ManagePlans");
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> EditPlan(int id)
        {
            var plan = await _membershipService.GetPlanForEditAsync(id);
            if (plan == null) return NotFound();

            return View(plan);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlan(SubscriptionPlanEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _membershipService.UpdatePlanAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Could not update the plan.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Plan updated successfully.";
            return RedirectToAction("ManagePlans");
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var (success, error) = await _membershipService.DeletePlanAsync(id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Plan deleted." : error;

            return RedirectToAction("ManagePlans");
        }
    }
}
