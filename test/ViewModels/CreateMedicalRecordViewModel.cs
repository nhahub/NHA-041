using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class CreateMedicalRecordViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int animalId { get; set; }

        [Required]
        [StringLength(100)]
        public string status { get; set; }

        [StringLength(500)]
        public string? injurys { get; set; }
    }
}
