using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using test.Services;
using test.ViewModels;
using test.Repository;
using test.Interfaces;
using Microsoft.AspNetCore.Identity;
using test.Repository;

namespace test.Controllers
{
    public class AnimalController : Controller
    {
        private readonly IAnimal _animalRepository;
        private readonly IRequests _requestRepository;
        private readonly PhotoServices _photoServices;
        private readonly UserManager<ApplicationUser> _userManager;
        public AnimalController(IAnimal animalRepository, IRequests requestRepository, PhotoServices photoServices, UserManager<ApplicationUser> userManager)
        {
            _animalRepository = animalRepository;
            _requestRepository = requestRepository;
            _photoServices = photoServices;
            _userManager = userManager;
        }
        
        [AllowAnonymous]
        public IActionResult Index(string? type, string? location, string? gender, bool mine)
        {
            // If anonymous user tries to access "My Animals", redirect to login
            if (mine && !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "Account", new { ReturnUrl = Url.Action("Index", "Animal", new { mine = true }) });
            }
            
            var userid = _userManager.GetUserId(User);
            var animalviewmodel = _animalRepository.AnimalDisplay(type, location, gender, userid, mine);
            ViewBag.Userid = userid;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            return View(animalviewmodel);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAnimalViewModel animalVM)
        {

            if (ModelState.IsValid)
            {
                var photoresult = await _photoServices.AddPhotoAsync(animalVM.Photo);
                var userid = _userManager.GetUserId(User);
                
                // Determine the final breed value
                string? finalBreed = animalVM.Breed;
                if (animalVM.Breed == "Other" || animalVM.Type == "Other")
                {
                    finalBreed = !string.IsNullOrWhiteSpace(animalVM.CustomBreed) ? animalVM.CustomBreed : null;
                }
                
                var animal = new Animal
                {
                    Name = animalVM.Name,
                    Type = animalVM.Type,
                    Age = animalVM.Age,
                    Photo = photoresult.Url.ToString(),
                    Breed = finalBreed,
                    Gender = animalVM.Gender,
                    About = animalVM.About,
                    Userid = userid
                };

                // Check for duplicate
                var existingAnimal = await _animalRepository.FindDuplicateAsync(animal.Name, animal.Type, animal.Age, userid);
                if (existingAnimal != null)
                {
                    // If duplicate found, redirect to Medical Record creation for the existing animal
                    return RedirectToAction("Create", "MedicalRecord", new { animalid = existingAnimal.AnimalId });
                }

                await _animalRepository.AddAnimal(animal);

                return RedirectToAction("Create", "MedicalRecord", new {animalid=animal.AnimalId});
            }
            return View(animalVM);
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return View("index");
            }
            var animalVM = new AnimalEditViewModel
            {
                Id = animal.AnimalId,
                Name = animal.Name,
                Type = animal.Type,
                Age = animal.Age,
                Gender = animal.Gender
            };
            return View(animalVM);

        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(AnimalEditViewModel animalVM)
        {
            if (ModelState.IsValid)
            {
                var animal = await _animalRepository.GetByIdAsync(animalVM.Id);
                if (animal == null)
                {
                    return View("Index");
                }
                animal.Name = animalVM.Name;
                animal.Type = animalVM.Type;
                animal.Age = animalVM.Age;
                animal.Gender = animalVM.Gender;
                if (animalVM.Photo != null)
                {
                    await _photoServices.DeletePhotoAsync(animal.Photo);
                    var photoresult = await _photoServices.AddPhotoAsync(animalVM.Photo);
                    animal.Photo = photoresult.Url.ToString();
                }
                bool mine = true;
                _animalRepository.savechanges();

                if (User.IsInRole("User"))
                {
                    return RedirectToAction("Index", new { mine = true });
                }
                else
                {
                    return RedirectToAction("Index", "Shelter");
                }
            }
            return View(animalVM);

        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var animal = await _animalRepository.GetByIdWithDetailsAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            
            var currentUserId = _userManager.GetUserId(User);
            ViewBag.IsOwner = User.Identity.IsAuthenticated && animal.Userid == currentUserId;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            
            // Check if user already sent a request for this animal
            ViewBag.HasPendingRequest = currentUserId != null && 
                await _requestRepository.HasPendingRequestForAnimal(currentUserId, id);
            
            return View(animal);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return View("Index");
            }
            await _photoServices.DeletePhotoAsync(animal.Photo);
            await _animalRepository.DeleteAnimal(animal);
            if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", new { mine = true });
            }
            else
            {
                return RedirectToAction("Index", "Shelter");
            }
        }
    }
}
