using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using test.Data;
using test.Interfaces;
using test.Models;
using test.ViewModels;
using System.Linq;

namespace test.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IContact _contactRepository;
        private readonly IAnimal _animalRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DepiContext _context;

        public AdminController(IContact contactRepository, IAnimal animalRepository, UserManager<ApplicationUser> userManager, DepiContext depi)
        {
            _contactRepository = contactRepository;
            _animalRepository = animalRepository;
            _userManager = userManager;
            _context = depi;
        }

        public async Task<IActionResult> Index()
        {
            // Get statistics
            var totalUsers = await _userManager.Users.CountAsync();
            var animalsAvailable = await _context.Animals.Where(a => !a.IsAdopted).CountAsync();
            var pendingRequests = await _context.Requests.Where(r => r.Status == "Pending").CountAsync();
            var totalMessages = await _context.ChatMessages.CountAsync();
            
            // Get recent messages (last 3, ordered by time descending)
            var recentMessages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.Time)
                .Take(3)
                .ToListAsync();
            
            // Get pending requests (last 3, ordered by most recent)
            var pendingRequestsList = await _context.Requests
                .Include(r => r.User)
                .Include(r => r.Animal)
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.Reqid)
                .Take(3)
                .ToListAsync();
            
            // Get users and animals for requests
            var requestUserIds = pendingRequestsList.Select(r => r.Userid).Distinct().ToList();
            var requestAnimalIds = pendingRequestsList.Select(r => r.AnimalId).Distinct().ToList();
            
            var requestUsers = await _userManager.Users
                .Where(u => requestUserIds.Contains(u.Id))
                .ToListAsync();
            
            var requestAnimals = await _context.Animals
                .Where(a => requestAnimalIds.Contains(a.AnimalId))
                .ToListAsync();
            
            // Calculate changes (simplified - you can add month comparison logic later)
            var usersThisMonth = await _userManager.Users
                .Where(u => u.Id != null) // Placeholder - add date comparison
                .CountAsync();
            
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                AnimalsAvailable = animalsAvailable,
                PendingRequests = pendingRequests,
                TotalMessages = totalMessages,
                UsersChange = "+12%", // Placeholder - calculate based on your needs
                AnimalsChange = "-3",
                RequestsChange = $"+{pendingRequests}",
                MessagesChange = "+28",
                RecentMessages = recentMessages,
                PendingRequestsList = pendingRequestsList,
                RequestUsers = requestUsers,
                RequestAnimals = requestAnimals
            };
            
            return View(viewModel);
        }

        public async Task<IActionResult> Animals()
        {
            var animals = await _animalRepository.GetAllAnimalsAsync();
            return View(animals);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditAnimal(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        [HttpPost]
        public async Task<IActionResult> EditAnimal(Animal animal)
        {
            if (ModelState.IsValid)
            {
                var existingAnimal = await _animalRepository.GetByIdAsync(animal.AnimalId);
                if (existingAnimal == null)
                {
                    return NotFound();
                }

                existingAnimal.Name = animal.Name;
                existingAnimal.Type = animal.Type;
                existingAnimal.Age = animal.Age;
                existingAnimal.Photo = animal.Photo;

                await _animalRepository.UpdateAnimal(existingAnimal);
                return RedirectToAction("Animals");
            }
            return View(animal);
        }

        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal != null)
            {
                await _animalRepository.DeleteAnimal(animal);
            }
            return RedirectToAction("Animals");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var orders = await _context.Orders.Where(o => o.UserId == id).ToListAsync();
                if (orders.Any())
                {
                    _context.Orders.RemoveRange(orders);
                    await _context.SaveChangesAsync();
                }
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> AllRequests()
        {
            var pendingRequests = await _context.Requests
                .Include(r => r.User)
                .Include(r => r.Animal)
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.Reqid)
                .ToListAsync();

            var requestUserIds = pendingRequests.Select(r => r.Userid).Distinct().ToList();
            var requestAnimalIds = pendingRequests.Select(r => r.AnimalId).Distinct().ToList();

            var requestUsers = await _userManager.Users
                .Where(u => requestUserIds.Contains(u.Id))
                .ToListAsync();

            var requestAnimals = await _context.Animals
                .Where(a => requestAnimalIds.Contains(a.AnimalId))
                .ToListAsync();

            ViewBag.RequestUsers = requestUsers;
            ViewBag.RequestAnimals = requestAnimals;

            return View(pendingRequests);
        }

        public async Task<IActionResult> AllMessages()
        {
            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.Time)
                .ToListAsync();

            return View(messages);
        }

        public async Task<IActionResult> ContactMessages()
        {
            var messages = await _contactRepository.GetAllMessagesAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AllRequests");
        }
    }
}
