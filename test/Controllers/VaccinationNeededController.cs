using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using test.Interfaces;
using test.Models;
using test.ViewModels;
using System.Linq;

namespace test.Controllers
{
    [Authorize]
    public class VaccinationNeededController : Controller
    {
        private readonly IVaccinationNeeded _vaccineRepo;

        public VaccinationNeededController(IVaccinationNeeded vaccineRepo)
        {
            _vaccineRepo = vaccineRepo;
        }

        public async Task<IActionResult> ByAnimal(int animalId)
        {
            var vaccines = await _vaccineRepo.GetByAnimalIdAsync(animalId);
            return View(vaccines);
        }

        public IActionResult Create(int medicalid)
        {
            var model = new CreateVaccinationViewModel
            {
                MedicalRecordId = medicalid
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVaccinationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NeedsVaccines && model.VaccineNames != null)
            {
                foreach (var name in model.VaccineNames)
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var vaccine = new VaccinationNeeded
                        {
                            Medicalid = model.MedicalRecordId,
                            VaccineName = name
                        };
                        await _vaccineRepo.AddAsync(vaccine);
                    }
                }
            }
            bool mine = true;
            if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Animal", new { mine = mine });
            }
            return RedirectToAction("Index", "Shelter", new { view = "animals" });
        }

        public IActionResult Delete(int id)
        {
            _vaccineRepo.Remove(id);
            if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Animal", new { mine = true });
            }
            return RedirectToAction("Index", "Shelter", new { view = "animals" });
        }
    }
}