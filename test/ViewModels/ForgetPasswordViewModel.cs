using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
    }
}
