using Microsoft.AspNetCore.Identity;
using test.Models;
using test.ViewModels;

namespace test.Interfaces
{
    public interface IAccounts
    {
        public  Task<ApplicationUser> GetUserbyid(string id);
        public List<ApplicationUser> GetUsers();
        public  Task<bool> adduser(registerviewmodel user);
        public bool removeuser(string id);
        public bool savechanges();
         public Task<ApplicationUser> SignIn(LoginViewModel user);
    }
}
