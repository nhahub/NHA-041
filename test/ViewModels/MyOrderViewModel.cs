using test.Models;

namespace test.ViewModels
{
    public class MyOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool OrderPaid { get; set; } // 0 = Pending, 1 = Paid/Done
        public decimal TotalPrice { get; set; }
        public string? PaymentMethodType { get; set; }
        public string? PaymentLast4Digits { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; } = new List<OrderDetailViewModel>();
        
        // Grouped items (combined by ProductId) - reuses GroupedItemViewModel
        public List<GroupedItemViewModel> GroupedOrderDetails { get; set; } = new List<GroupedItemViewModel>();
    }

    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductType { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductPhoto { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ShelterName { get; set; }
        public string? ShelterUserId { get; set; }
        public string? ShelterEmail { get; set; }
        public string? ShelterPhone { get; set; }
    }

    public class MyOrdersPageViewModel
    {
        public List<MyOrderViewModel> Orders { get; set; } = new List<MyOrderViewModel>();
        public int TotalOrders => Orders.Count;
        public int CompletedOrders => Orders.Count(o => o.OrderPaid == true);
        public int PendingOrders => Orders.Count(o => o.OrderPaid == false);
    }
}

