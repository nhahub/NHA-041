using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        private readonly DepiContext _context;
        private readonly test.Interfaces.IContact _contactRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(test.Interfaces.IContact contactRepository, UserManager<ApplicationUser> userManager)
        {
            _context = new DepiContext();
            _contactRepository = contactRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult FAQs()
        {
            return View();
        }
        public async Task<IActionResult> contactus()
        {
            var model = new ContactMessage();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.Name = user.UserName;
                    model.Email = user.Email;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(ContactMessage message1)
        {
            if (ModelState.IsValid)
            {
                await _contactRepository.AddMessageAsync(message1);
                TempData["SuccessMessage"] = "Your message has been sent successfully!";
                return RedirectToAction("contactus");
            }
            return View("contactus", message1);
        }
    }
}
