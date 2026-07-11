using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Membership;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PricingPageViewModel> GetPricingPageAsync(string? currentUserId)
        {
            var plans = await GetAllPlansAsync();

            SubscriptionPlanViewModel? activePlan = null;

            if (!string.IsNullOrEmpty(currentUserId))
            {
                var activeSub = await _unitOfWork.UserSubscriptionRepository.Query()
                    .Include(s => s.SubscriptionPlan)
                    .Where(s => s.UserId == currentUserId && s.IsActive && s.EndDate > DateTime.UtcNow)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (activeSub != null)
                {
                    activePlan = MapToViewModel(activeSub.SubscriptionPlan);
                }
            }

            return new PricingPageViewModel
            {
                Plans = plans,
                CurrentUserActivePlan = activePlan
            };
        }

        public async Task<List<SubscriptionPlanViewModel>> GetAllPlansAsync()
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();
            return plans.OrderBy(p => p.Price).Select(MapToViewModel).ToList();
        }

        public async Task<(bool Success, string? Error)> SubscribeAsync(string userId, int planId)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);
            if (plan == null)
            {
                return (false, "Plan not found.");
            }

            // Deactivate any currently active subscription before starting a new one.
            var activeSubs = await _unitOfWork.UserSubscriptionRepository
                .FindAsync(s => s.UserId == userId && s.IsActive);

            foreach (var sub in activeSubs)
            {
                sub.IsActive = false;
                _unitOfWork.UserSubscriptionRepository.Update(sub);
            }

            // NOTE for graduation defense: this is where a real payment
            // gateway call (Stripe/PayPal) would happen before confirming
            // the subscription. Kept as a direct confirmation here to stay
            // in scope for a student project.
            var subscription = new UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = planId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays),
                IsActive = true
            };

            await _unitOfWork.UserSubscriptionRepository.AddAsync(subscription);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<MySubscriptionViewModel> GetMySubscriptionAsync(string userId)
        {
            var activeSub = await _unitOfWork.UserSubscriptionRepository.Query()
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            if (activeSub == null)
            {
                return new MySubscriptionViewModel { HasActiveSubscription = false };
            }

            return new MySubscriptionViewModel
            {
                HasActiveSubscription = true,
                PlanName = activeSub.SubscriptionPlan.Name,
                StartDate = activeSub.StartDate,
                EndDate = activeSub.EndDate
            };
        }

        public async Task<(bool Success, string? Error)> CreatePlanAsync(SubscriptionPlanEditViewModel model)
        {
            var plan = new SubscriptionPlan
            {
                Name = model.Name,
                Price = model.Price,
                DurationInDays = model.DurationInDays,
                Features = model.Features,
                IsFeatured = model.IsFeatured
            };

            await _unitOfWork.SubscriptionPlanRepository.AddAsync(plan);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdatePlanAsync(SubscriptionPlanEditViewModel model)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(model.Id);
            if (plan == null)
            {
                return (false, "Plan not found.");
            }

            plan.Name = model.Name;
            plan.Price = model.Price;
            plan.DurationInDays = model.DurationInDays;
            plan.Features = model.Features;
            plan.IsFeatured = model.IsFeatured;

            _unitOfWork.SubscriptionPlanRepository.Update(plan);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeletePlanAsync(int planId)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);
            if (plan == null)
            {
                return (false, "Plan not found.");
            }

            var hasSubscribers = (await _unitOfWork.UserSubscriptionRepository
                .FindAsync(s => s.SubscriptionPlanId == planId)).Any();

            if (hasSubscribers)
            {
                return (false, "This plan has active or past subscribers and cannot be deleted.");
            }

            _unitOfWork.SubscriptionPlanRepository.Delete(plan);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<SubscriptionPlanEditViewModel?> GetPlanForEditAsync(int planId)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);
            if (plan == null) return null;

            return new SubscriptionPlanEditViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                Features = plan.Features,
                IsFeatured = plan.IsFeatured
            };
        }

        public async Task<SubscriptionPlanViewModel?> GetPlanAsync(int planId)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);

            if (plan == null)
                return null;

            return new SubscriptionPlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                Features = plan.Features,
                IsFeatured = plan.IsFeatured
            };
        }
        private static SubscriptionPlanViewModel MapToViewModel(SubscriptionPlan p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            DurationInDays = p.DurationInDays,
            Features = p.Features,
            IsFeatured = p.IsFeatured
        };
    }
}
