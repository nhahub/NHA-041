using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class AnimalEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Range(0, 30)]
        public byte? Age { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
