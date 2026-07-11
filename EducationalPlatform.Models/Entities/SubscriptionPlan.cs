using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationalPlatform.Models.Entities
{
    /// <summary>
    /// A purchasable membership tier (mirrors the 3 pricing cards in pricing.html:
    /// Basic/Standard/Premium). Kept intentionally simple - no real payment
    /// gateway integration, just a "subscribe" confirmation flow suitable for
    /// a graduation project. A real implementation would call out to
    /// Stripe/PayPal from MembershipController.Subscribe before saving.
    /// </summary>
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        /// <summary>Billing period in days (e.g. 30 for monthly).</summary>
        public int DurationInDays { get; set; } = 30;

        [MaxLength(1000)]
        public string? Features { get; set; } // simple newline/semicolon separated list for display

        public bool IsFeatured { get; set; } = false;

        public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
    }
}
