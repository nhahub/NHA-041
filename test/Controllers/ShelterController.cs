using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using test.Data;
using test.Interfaces;
using test.Models;
using test.ViewModels;
using test.Services;

namespace test.Controllers
{
    public class ShelterController : Controller
    {
        private readonly DepiContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IShelter _ShelterRepository;
        private readonly IAnimal _animalRepository;
        private readonly PhotoServices _photoServices;

        public ShelterController(DepiContext context, UserManager<ApplicationUser> userManager, IShelter shelter, IAnimal animalRepository, PhotoServices photoServices)
        {
            _usermanager = userManager;
            _context = context;
            _ShelterRepository = shelter;
            _animalRepository = animalRepository;
            _photoServices = photoServices;
        }

        [Authorize(Roles = "Shelter")]
        public async Task<IActionResult> Index(string view)
        {
            var userid = _usermanager.GetUserId(User);
            List<Animal> animals=null;
            List<Product> products=null;
            if (view == "animals")
            {
                 animals = await _animalRepository.GetAllUserAnimalsAsync(userid);
            }
            else
            {
                 products = await _ShelterRepository.GetAllProducts(userid);
            }
            

            var viewModel = new ShelterIndexViewModel
            {
                Products = products,
                Animals = animals
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Shelter")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Shelter")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userid = _usermanager.GetUserId(User);
                string? photoUrl = null;
                
                if (model.Photo != null)
                {
                    var uploadResult = await _photoServices.AddPhotoAsync(model.Photo);
                    if (uploadResult.Error == null)
                    {
                        photoUrl = uploadResult.SecureUrl.ToString();
                    }
                }
                
                var product = new Product
                {   Name=model.Name,
                    Type = model.Type,
                    Disc = model.Disc,
                    Price = model.Price,
                    Userid = userid,
                    Quantity = model.Quantity,
                    Photo = photoUrl
                };
                await _ShelterRepository.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> userview()
        {
            var shlters = await _ShelterRepository.GetAllShelters();
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            return View(shlters);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Shelterpage(ApplicationUser shelter)
        {
            // Fetch the full shelter user to get all properties including location
            var shelterUser = await _usermanager.FindByIdAsync(shelter.Id);
            
            var products = await _ShelterRepository.GetAllProducts(shelter.Id);
            var animals = await _animalRepository.GetAllUserAnimalsAsync(shelter.Id);
            var userid = _usermanager.GetUserId(User);
            ViewBag.userid = userid;
            ViewBag.ShelterId = shelter.Id;
            ViewBag.email = shelter.Email;
            ViewBag.phonenumber = shelter.PhoneNumber;
            ViewBag.username = shelterUser?.FullName ?? shelter.UserName;
            ViewBag.location = shelterUser?.location;

            var viewModel = new ShelterIndexViewModel
            {
                Products = products,
                Animals = animals
            };
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            return View(viewModel);
        }
        
        [Authorize(Roles = "Shelter")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _ShelterRepository.GetProductbyId(id);

            if (product == null)
            {
                return NotFound();
            }
            var editModel = new EditProductViewModel
            {
                ProductId = product.Productid,
                Type = product.Type,
                Price = product.Price,
                Quantity = product.Quantity ,
                Disc = product.Disc,
                CurrentPhotoUrl = product.Photo,
                Name = product.Name
            };
            return View(editModel);
        }

        [Authorize(Roles = "Shelter")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _ShelterRepository.GetProductbyId(model.ProductId);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Only update allowed fields (not Type)
                existingProduct.Price = model.Price;
                existingProduct.Quantity = model.Quantity;
                existingProduct.Disc = model.Disc;
                existingProduct.Name = model.Name;

                // Handle photo upload
                if (model.Photo != null)
                {
                    var uploadResult = await _photoServices.AddPhotoAsync(model.Photo);
                    if (uploadResult.Error == null)
                    {
                        existingProduct.Photo = uploadResult.SecureUrl.ToString();
                    }
                }

                await _ShelterRepository.UpdateProduct(existingProduct);
                return RedirectToAction("Index");
            }
            
            // Re-populate CurrentPhotoUrl if validation fails
            var product = await _ShelterRepository.GetProductbyId(model.ProductId);
            model.CurrentPhotoUrl = product?.Photo;
            return View(model);
        }

        [Authorize(Roles = "Shelter")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _ShelterRepository.GetProductbyId(id);
            if (product != null)
            {
                await _ShelterRepository.RemoveProduct(product);
            }
            return RedirectToAction("Index");
        }
    }
}
