
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Interfaces;
using test.Models;

namespace test.Repository
{
    public class MedicalRecordRepository : IMedicalRecord
    {
        private readonly DepiContext _context;

        public MedicalRecordRepository(DepiContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecord?> GetByAnimalIdAsync(int animalId)
        {
            return await _context.MedicalRecords
                .Include(r => r.VaccinationNeededs)
                .FirstOrDefaultAsync(r => r.Animalid == animalId);
        }

        public List<MedicalRecord> GetAll()
        {
            return _context.MedicalRecords
                .Include(r => r.VaccinationNeededs)
                .ToList();
        }

        public async Task<bool> AddAsync(MedicalRecord record)
        {
            await _context.MedicalRecords.AddAsync(record);
            return SaveChanges();
        }

        public async Task<bool> Update(MedicalRecord record)
        {
            _context.MedicalRecords.Update(record);
            return SaveChanges();
        }

        public bool RemoveByAnimalId(int animalId)
        {
            var record = _context.MedicalRecords.FirstOrDefault(r => r.Animalid == animalId);
            if (record == null) return false;

            _context.MedicalRecords.Remove(record);
            return SaveChanges();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
