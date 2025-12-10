using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class CreatePaymentMethodViewModel
    {
        [Required]
        public string Nonce { get; set; }  // This comes from Braintree Drop-in
    }
}
