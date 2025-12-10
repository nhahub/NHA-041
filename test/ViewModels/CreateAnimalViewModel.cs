using System.ComponentModel.DataAnnotations;

namespace test.ViewModels
{
    public class CreateAnimalViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(1, 30, ErrorMessage = "Age must be between 1 and 30")]
        public byte Age { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(100)]
        public string? Breed { get; set; }

        [StringLength(100)]
        public string? CustomBreed { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(500)]
        public string? About { get; set; }

        [Required]
        public IFormFile Photo { get; set; }
    }
}
