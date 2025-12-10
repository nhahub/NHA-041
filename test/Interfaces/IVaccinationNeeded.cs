using System.Collections.Generic;
using System.Threading.Tasks;
using test.Models;

namespace test.Interfaces
{
    public interface IVaccinationNeeded
    {
        Task<List<VaccinationNeeded>> GetByAnimalIdAsync(int animalId);
        Task<List<VaccinationNeeded>> GetByMedicalRecordIdAsync(int recordId);
        Task<bool> AddAsync(VaccinationNeeded vaccine);
        Task<bool> UpdateAsync(VaccinationNeeded vaccine);
        bool Remove(int id);
        bool SaveChanges();
    }
}