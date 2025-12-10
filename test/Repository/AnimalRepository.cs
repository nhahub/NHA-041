using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Interfaces;
using test.Models;
using test.ViewModels;

namespace test.Repository
{
    public class AnimalRepository:IAnimal
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly DepiContext _context;

        public AnimalRepository(DepiContext context,UserManager<ApplicationUser> usermanager) { 
            _usermanager = usermanager;
        
        _context= context;
        }

        public async Task<bool> AddAnimal(Animal animal)
        {
            await _context.Animals.AddAsync(animal);
            return savechanges();
        }

        public Animalviewmodel AnimalDisplay(string? typeFilter, string? locationFilter, string? genderFilter, string userid,bool mine)
        {
            // Base query: no tracking for lighter read
            IQueryable<Animal> animals = _context.Animals
                .AsNoTracking()
                .Include(a => a.User);

            if (mine)
            {
                // For "My Animals" we need medical records to show the Record button
                animals = animals
                    .Include(a => a.MedicalRecords)
                    .Where(anm => anm.Userid == userid);
            }
            else
            {
                animals = animals.Where(a =>
                    a.Userid != userid &&
                    !a.IsAdopted &&
                    !_context.Requests.Any(r => r.Userid == userid && r.AnimalId == a.AnimalId));
            }

            // Apply filters for both browsing and "My" view
            var normalizedType = string.IsNullOrWhiteSpace(typeFilter) ? "any" : typeFilter.ToLower();
            var normalizedGender = string.IsNullOrWhiteSpace(genderFilter) ? "any" : genderFilter.ToLower();
            var normalizedLocation = string.IsNullOrWhiteSpace(locationFilter) ? "any" : locationFilter;

            if (normalizedType != "any")
            {
                animals = animals.Where(a => a.Type != null && a.Type.ToLower() == normalizedType);
            }

            if (normalizedGender != "any")
            {
                animals = animals.Where(a => a.Gender != null && a.Gender.ToLower() == normalizedGender);
            }

            if (normalizedLocation != "any")
            {
                animals = animals.Where(a => a.User != null && a.User.location == normalizedLocation);
            }

            // Build filter option lists
            var typeOptions = _context.Animals
                .Where(a => a.Type != null && a.Type != "")
                .Select(a => a.Type)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            var locationOptions = _context.Users
                .Where(u => !string.IsNullOrWhiteSpace(u.location))
                .Select(u => u.location!)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            var animviewmodel = new Animalviewmodel
            {
                animals = animals.ToList(),
                TypeFilter = normalizedType,
                GenderFilter = normalizedGender,
                LocationFilter = normalizedLocation == "any" ? null : normalizedLocation,
                TypeOptions = typeOptions,
                LocationOptions = locationOptions,
                IsMine = mine
            };

            return animviewmodel;
        }

        public bool savechanges()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public async Task<Animal> GetByIdAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.AnimalId == id);
            return  animal;
        }

        public async Task<Animal?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Animals
                .Include(a => a.User)
                .Include(a => a.MedicalRecords)
                    .ThenInclude(m => m.VaccinationNeededs)
                .FirstOrDefaultAsync(a => a.AnimalId == id);
        }
        public async Task<bool> DeleteAnimal(Animal animal)
        {
            _context.Animals.Remove(animal);
            return savechanges();
        }
        public Task<List<Animal>> GetAllUserAnimalsAsync(string id)
        {
            var animals = _context.Animals
                .Include(a => a.MedicalRecords)
                .Where(a => a.Userid == id)
                .ToListAsync();
            return animals;
        }
        public async Task<List<Animal>> GetAllAnimalsAsync() { 
            var animals= await _context.Animals.ToListAsync();
            return animals;
        }

        public async Task<bool> UpdateAnimal(Animal animal)
        {
            _context.Animals.Update(animal);
            return savechanges();
        }

        public async Task<Animal?> FindDuplicateAsync(string name, string type, byte? age, string userId)
        {
            return await _context.Animals
                .FirstOrDefaultAsync(a => a.Name == name && a.Type == type && a.Age == age && a.Userid == userId);
        }
        public string GetAnimalOwnerId(int animalId)
        {
                var animalOwnerId = _context.Animals
                .Where(a => a.AnimalId == animalId)
                .Select(a => a.Userid)
                .FirstOrDefault();
            if (animalOwnerId != null)
                {
                    return animalOwnerId;
                }

            return null; // or throw an exception if preferred
        }
    }
}
