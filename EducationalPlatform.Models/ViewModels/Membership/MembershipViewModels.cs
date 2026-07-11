using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Membership
{
    public class SubscriptionPlanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public string? Features { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class PricingPageViewModel
    {
        public List<SubscriptionPlanViewModel> Plans { get; set; } = new();
        public SubscriptionPlanViewModel? CurrentUserActivePlan { get; set; }
    }

    public class SubscriptionPlanEditViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100), Display(Name = "Plan Name")]
        public string Name { get; set; } = string.Empty;

        [Required, Range(0, 10000), Display(Name = "Price (USD)")]
        public decimal Price { get; set; }

        [Range(1, 3650), Display(Name = "Duration (days)")]
        public int DurationInDays { get; set; } = 30;

        [Display(Name = "Features (one per line)")]
        public string? Features { get; set; }

        [Display(Name = "Mark as Featured / Best Value")]
        public bool IsFeatured { get; set; }
    }

    public class MySubscriptionViewModel
    {
        public bool HasActiveSubscription { get; set; }
        public string? PlanName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
