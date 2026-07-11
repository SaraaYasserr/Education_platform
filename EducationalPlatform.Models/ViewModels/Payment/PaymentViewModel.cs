using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.Models.ViewModels.Payment
{
    public class PaymentViewModel
    {
        public int? CourseId { get; set; }

        public int? PlanId { get; set; }

        public string? ItemName { get; set; }

        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required]
        public string CVV { get; set; } = string.Empty;

        public bool IsSubscription { get; set; }
    }
}