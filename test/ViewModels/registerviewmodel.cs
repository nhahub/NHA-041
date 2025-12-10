using Microsoft.AspNetCore.Http;

namespace test.ViewModels
{
    public class registerviewmodel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phonenumber { get; set; }
        public string role { get; set; } = "User";
        public IFormFile? Photo { get; set; }
        public string? FullName { get; set; }


        public string? Location { get; set; }
    }
}
