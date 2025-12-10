using test.Models;
using test.ViewModels;

namespace test.Interfaces
{
    public interface IAnimal
    {
        public  Animalviewmodel AnimalDisplay(string? typeFilter, string? locationFilter, string? genderFilter, string id,bool mine);
        public Task<Animal> GetByIdAsync(int id);
        public Task<Animal?> GetByIdWithDetailsAsync(int id);
        public Task<List<Animal>> GetAllUserAnimalsAsync(string id);
        public Task<List<Animal>> GetAllAnimalsAsync();
        public Task<bool> UpdateAnimal(Animal animal);
        public Task<bool> DeleteAnimal(Animal animal);
        public Task<bool> AddAnimal(Animal animal);
        public Task<Animal?> FindDuplicateAsync(string name, string type, byte? age, string userId);
        public string GetAnimalOwnerId(int animalId);
        public bool savechanges();
    }
}
