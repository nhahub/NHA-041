using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Interfaces;
using test.Models;

namespace test.Repository
{
    public class VaccinationNeededRepository : IVaccinationNeeded
    {
        private readonly DepiContext _context;

        public VaccinationNeededRepository(DepiContext context)
        {
            _context = context;
        }


        public async Task<List<VaccinationNeeded>> GetByAnimalIdAsync(int animalId)
        {
            return await _context.VaccinationNeededs
                .Include(v => v.MedicalRecord)
                .Where(v => v.MedicalRecord.Animalid == animalId)
                .ToListAsync();
        }


        public async Task<List<VaccinationNeeded>> GetByMedicalRecordIdAsync(int recordId)
        {
            return await _context.VaccinationNeededs
                .Where(v => v.Medicalid == recordId)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(VaccinationNeeded vaccine)
        {
            await _context.VaccinationNeededs.AddAsync(vaccine);
            return SaveChanges();
        }

        public async Task<bool> UpdateAsync(VaccinationNeeded vaccine)
        {
            _context.VaccinationNeededs.Update(vaccine);
            return SaveChanges();
        }

        public bool Remove(int id)
        {
            var item = _context.VaccinationNeededs.FirstOrDefault(v => v.Id == id);
            if (item == null) return false;

            _context.VaccinationNeededs.Remove(item);
            return SaveChanges();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
