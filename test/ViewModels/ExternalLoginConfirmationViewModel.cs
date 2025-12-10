using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Select Role")]
        public string Role { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }
    }
}
