using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test.Data;
using System.Security.Claims;
using test.Models;
using Microsoft.AspNetCore.Authentication;
using test.ViewModels;
using test.Repository;
using test.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace test.Repository
{
    public class AccountRepository : IAccounts
    {
        private readonly DepiContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountRepository(DepiContext context,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager) {
        
        _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public async Task<bool> adduser(registerviewmodel user)
        {
  
                return savechanges();
        }

        public async Task<ApplicationUser> GetUserbyid(string id)
        {
            
            return await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        }

        public List<ApplicationUser> GetUsers()
        {
            throw new NotImplementedException();
        }

        public bool removeuser(string id)
        {
            throw new NotImplementedException();
        }

        public bool savechanges()
        {
            var saved=_context.SaveChanges();
            


            return saved > 0 ? true : false;
        }

        public async Task<ApplicationUser> SignIn(LoginViewModel user)
        {
            return await userManager.Users.FirstOrDefaultAsync(u=>u.Email==user.email&&u.Email==user.password);
            
        }

    }
}
