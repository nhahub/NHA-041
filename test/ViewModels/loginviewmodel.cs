using Microsoft.AspNetCore.Authentication;

namespace test.ViewModels
{
    public class LoginViewModel
    {
        public string email { get; set; }
        public string password { get; set; }    

        public string ?returnUrl { get; set; }

        public IList<AuthenticationScheme> ?ExternalLogins { get; set; } 
    }
}
