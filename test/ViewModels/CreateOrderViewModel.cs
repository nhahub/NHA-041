using System.ComponentModel.DataAnnotations;
using test.Models;

namespace test.ViewModels
{
    public class CreateOrderViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        public string? UserId { get; set; }

        [StringLength(100)]
        public string? productName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public decimal productPrice { get; set; }
    }
}
