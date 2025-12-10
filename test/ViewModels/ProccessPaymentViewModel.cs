using System.ComponentModel.DataAnnotations;
using test.Models;

namespace test.ViewModels
{
    public class ProccessPaymentViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int orderid { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public decimal totalprice { get; set; }
        public List<OrderDetails>? orderDetails { get; set; }
        public List<PaymentMethods>? paymentMethods { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int selectedPaymentMethodid { get; set; }
        
        // Grouped items for order summary
        public List<GroupedItemViewModel> GroupedItems { get; set; } = new();
        [Range(0, int.MaxValue)]
        public decimal TotalQuantity { get; set; }
    }
}
