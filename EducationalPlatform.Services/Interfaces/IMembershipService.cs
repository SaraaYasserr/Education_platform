using EducationalPlatform.Models.ViewModels.Membership;

namespace EducationalPlatform.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<PricingPageViewModel> GetPricingPageAsync(string? currentUserId);
        Task<List<SubscriptionPlanViewModel>> GetAllPlansAsync();
        Task<(bool Success, string? Error)> SubscribeAsync(string userId, int planId);
        Task<MySubscriptionViewModel> GetMySubscriptionAsync(string userId);

        // Admin management of plans
        Task<(bool Success, string? Error)> CreatePlanAsync(SubscriptionPlanEditViewModel model);
        Task<(bool Success, string? Error)> UpdatePlanAsync(SubscriptionPlanEditViewModel model);
        Task<(bool Success, string? Error)> DeletePlanAsync(int planId);
        Task<SubscriptionPlanEditViewModel?> GetPlanForEditAsync(int planId);
        Task<SubscriptionPlanViewModel?> GetPlanAsync(int planId);
    }
}
