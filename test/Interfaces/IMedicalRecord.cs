using System.Collections.Generic;
using System.Threading.Tasks;
using test.Models;

namespace test.Interfaces
{
    public interface IMedicalRecord
    {
        public Task<MedicalRecord?> GetByAnimalIdAsync(int animalId);
        public List<MedicalRecord> GetAll();
        public Task<bool> AddAsync(MedicalRecord record);
        public Task<bool> Update(MedicalRecord record);
        public bool RemoveByAnimalId(int animalId);
        public bool SaveChanges();
    }
}