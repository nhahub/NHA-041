using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace test.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Orders ?Order { get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")]
        public Product ?Product { get; set; }
       
       [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
