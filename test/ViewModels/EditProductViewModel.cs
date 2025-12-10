using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace test.ViewModels
{
    public class EditProductViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        public string? Type { get; set; } // Read-only, not editable
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Disc { get; set; }
        
        public IFormFile? Photo { get; set; } // New photo upload
        
        public string? CurrentPhotoUrl { get; set; } // Current photo to display
    }
}
