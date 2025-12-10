using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class CreateVaccinationViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int MedicalRecordId { get; set; }

        [Display(Name = "Does the animal need vaccines?")]
        public bool NeedsVaccines { get; set; }

        public List<string> VaccineNames { get; set; } = new List<string>();
    }
}
