using Microsoft.AspNetCore.Identity;
using test.Models;

namespace test.ViewModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Animal> Animals { get; set; }
        public string Role { get; set; }
        public bool IsOwner { get; set; }
        public bool CanViewContactInfo { get; set; }
    }
}
