namespace EducationalPlatform.Models.Entities
{
    public class UserSubscription
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int SubscriptionPlanId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    }
}
