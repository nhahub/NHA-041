using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(600)]
        [Display(Name = "City")]
        public string? Location { get; set; }

        [Display(Name = "Profile Photo")]
        public IFormFile? Photo { get; set; }

        public string? CurrentPhotoUrl { get; set; }
    }
}

