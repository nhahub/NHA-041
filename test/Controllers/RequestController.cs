using Microsoft.AspNetCore.Authentication;
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

namespace test.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IRequests _RequestRepository;
        private readonly IAnimal _animalRepository;

        public RequestController(IRequests RequestsRepository, UserManager<ApplicationUser> userManager, IAnimal animalRepository)
        {
            _usermanager = userManager;
            _RequestRepository = RequestsRepository;
            _animalRepository = animalRepository;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _usermanager.GetUserId(User);
            var requests = await _RequestRepository.LoadRequests(currentUserId);
            var animals = _RequestRepository.AnimalsNeeded(currentUserId, requests);
            var usersrequested = _RequestRepository.RequestGot(currentUserId, requests);
            var users = _RequestRepository.RequestSent(currentUserId, requests);

            // ViewBag items for the view
            ViewBag.usersrequested = usersrequested;
            ViewBag.animals = animals;
            ViewBag.users = users;
            ViewBag.userid = currentUserId;

            var viewModel = new RequestIndexViewModel
            {
                // Incoming Pending: Others want to adopt MY animals (I am the owner - Useridreq)
                IncomingPending = requests
                    .Where(r => r.Useridreq == currentUserId && r.Status == "Pending").Select(o=>new Request
                    {
                        Reqid = o.Reqid,
                        Userid =o.Userid,
                        AnimalId=o.AnimalId
                    })
                    .ToList(),

                // Incoming Approved: Others want to adopt MY animals - Approved
                IncomingApproved = requests
                    .Where(r => r.Useridreq == currentUserId && r.Status == "approved").Select(o=>new Request
                    {
                        Reqid = o.Reqid,
                        AnimalId =o.AnimalId,
                        Userid=o.Userid
                    })
                    .ToList(),

                // Outgoing Pending: I want to adopt others' animals - Pending
                OutgoingPending = requests
                    .Where(r => r.Userid == currentUserId && r.Useridreq != currentUserId && r.Status == "Pending").Select(o => new Request
                    {
                        Reqid = o.Reqid,
                        AnimalId = o.AnimalId,
                        Useridreq=o.Useridreq
                    })
                    .ToList(),

                // Outgoing Approved: I want to adopt others' animals - Approved
                OutgoingApproved = requests
                    .Where(r => r.Userid == currentUserId && r.Useridreq != currentUserId && r.Status == "approved").Select(o => new Request
                    {
                        AnimalId = o.AnimalId,
                        Useridreq = o.Useridreq,
                        Reqid = o.Reqid
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Request request)
        {
            if (ModelState.IsValid)
            {
                // Check if animal is already adopted
                var animal = await _animalRepository.GetByIdAsync(request.AnimalId);
                if (animal == null)
                {
                    return Json(new { success = false, message = "Animal not found." });
                }
                
                if (animal.IsAdopted)
                {
                    return Json(new { success = false, message = "This animal has already been adopted." });
                }

                await _RequestRepository.addRequest(request);
                return Json(new { success = true, message = "Adoption request sent successfully!", animalId = request.AnimalId });
            }
            return Json(new { success = false, message = "Invalid request." });
        }

        [HttpPost]
        public async Task<IActionResult> approve(int id)
        {
            if (await _RequestRepository.approverequest(id))
                return RedirectToAction("Index");
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> reject(int id)
        {
            if (await _RequestRepository.rejectRequest(id))
                return RedirectToAction("Index");
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var request = await _RequestRepository.GetRequestById(id);
            if (request != null)
            {
                await _RequestRepository.DeleteRequest(request);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAdoption(int id)
        {
            var request = await _RequestRepository.GetRequestById(id);
            if (request != null)
            {
                var animal = await _animalRepository.GetByIdAsync(request.AnimalId);
                var animalId = request.AnimalId;
                
                if (animal != null)
                {
                    animal.IsAdopted = true;
                    await _animalRepository.UpdateAnimal(animal);
                }

                // Delete all requests for this animal after completing adoption
                var requests = await _RequestRepository.LoadRequestsForAnimal(animalId);
                foreach (var req in requests)
                {
                    await _RequestRepository.DeleteRequest(req);
                }
                return Json(new { success = true, message = "Adoption completed successfully! The animal has found a new home.", animalId = animalId });
            }
            return Json(new { success = false, message = "Request not found." });
        }
    }
}
