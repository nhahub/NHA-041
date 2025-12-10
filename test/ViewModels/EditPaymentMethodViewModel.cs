using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class EditPaymentMethodViewModel
    {
        [Required]
        public int PaymentMethodId { get; set; }
        [Required(ErrorMessage = "Expiry Month is required")]
        [Range(1, 12, ErrorMessage = "Invalid month")]
        [Display(Name = "Expiry Month")]
        public int ExpiryMonth { get; set; }
        [Required(ErrorMessage = "Expiry Year is required")]
        [Range(2024, 2100)]
        [Display(Name = "Expiry Year")]
        public int ExpiryYear { get; set; }

    }
}
