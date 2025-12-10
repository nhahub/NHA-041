using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using test.Interfaces;
using test.Models;
using test.ViewModels;
using test.Services;

namespace test.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAnimal _animalRepository;
        private readonly IRequests _requestRepository;
        private readonly PhotoServices _photoServices;

        public ProfileController(UserManager<ApplicationUser> userManager, IAnimal animalRepository, IRequests requestRepository, PhotoServices photoServices)
        {
            _userManager = userManager;
            _animalRepository = animalRepository;
            _requestRepository = requestRepository;
            _photoServices = photoServices;
        }

        public async Task<IActionResult> Index(string? userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            ApplicationUser targetUser;
            bool isOwner = false;
            bool canViewContactInfo = false;

            if (string.IsNullOrEmpty(userId) || userId == currentUser.Id)
            {
                targetUser = currentUser;
                isOwner = true;
                canViewContactInfo = true;
            }
            else
            {
                targetUser = await _userManager.FindByIdAsync(userId);
                if (targetUser == null) return NotFound();

                isOwner = false;
                // Check if there is an accepted request between current user and target user
                canViewContactInfo = await _requestRepository.HasAcceptedRequest(currentUser.Id, targetUser.Id);
            }

            var roles = await _userManager.GetRolesAsync(targetUser);
            var role = roles.FirstOrDefault() ?? "User";

            var animals = await _animalRepository.GetAllUserAnimalsAsync(targetUser.Id);

            var viewModel = new UserProfileViewModel
            {
                User = targetUser,
                Animals = animals,
                Role = role,
                IsOwner = isOwner,
                CanViewContactInfo = canViewContactInfo
            };

            // Pass IsOwner to ViewData for the partial view
            ViewData["IsOwner"] = isOwner;
            ViewData["TargetUserId"] = targetUser.Id; // Needed for AJAX filter

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FilterAnimals(string query, string? userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            string targetUserId = userId ?? currentUser.Id;
            
            
            var animals = await _animalRepository.GetAllUserAnimalsAsync(targetUserId);

            if (!string.IsNullOrEmpty(query))
            {
                animals = animals.Where(a => 
                    (a.Name != null && a.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) || 
                    (a.Type != null && a.Type.Contains(query, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Determine ownership for the partial view
            bool isOwner = targetUserId == currentUser.Id;
            ViewData["IsOwner"] = isOwner;

            return PartialView("_AnimalListPartial", animals);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var viewModel = new EditProfileViewModel
            {
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                CurrentPhotoUrl = currentUser.PhotoUrl,
                FullName = currentUser.FullName,
                Location = currentUser.location
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.CurrentPhotoUrl = currentUser.PhotoUrl;
                return View(model);
            }

            // Update user fields
            currentUser.UserName = model.UserName;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.FullName = model.FullName;
            currentUser.location = model.Location;

            // Handle photo upload
            if (model.Photo != null && model.Photo.Length > 0)
            {
                var uploadResult = await _photoServices.AddPhotoAsync(model.Photo);
                if (uploadResult.Error == null)
                {
                    currentUser.PhotoUrl = uploadResult.SecureUrl.ToString();
                }
            }

            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.CurrentPhotoUrl = currentUser.PhotoUrl;
            return View(model);
        }
    }
}
