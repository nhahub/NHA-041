using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace test.ViewModels
{
    public class CreateProductViewModel
    {

       
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters.")]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Disc { get; set; }
        
        public IFormFile? Photo { get; set; }
    }
}
