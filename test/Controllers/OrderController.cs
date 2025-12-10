using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ServiceStack;
using test.Data;
using test.Interfaces;
using test.Models;
using test.ViewModels;

namespace test.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IShelter _shelterRepository;
        private readonly IOrder _orderRepository;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly DepiContext _context;
        public OrderController(IShelter shelterRepository, IOrder orderRepository, UserManager<ApplicationUser> userManager,DepiContext depiContext)
        {
            _shelterRepository = shelterRepository;
            _orderRepository = orderRepository;
            _usermanager = userManager;
            _context = depiContext;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create([FromForm] CreateOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByIdAsync(model.UserId);
                var product = await _shelterRepository.GetProductbyId(model.ProductId);
                if (user == null || product == null)
                {
                    return Json(new { Message = "error" });
                }
                var orderExists = await _context.Orders
                    .FirstOrDefaultAsync(o => o.UserId == user.Id && o.OrderPaid == false);
                if (orderExists == null)
                {
                     orderExists = new Orders
                    {
                        UserId = user.Id,
                        OrderDate = DateTime.Now,
                        OrderPaid = false
                    };
                    await _context.Orders.AddAsync(orderExists);
                    await _context.SaveChangesAsync();
                }
                var orderdetails = new OrderDetails
                {
                    OrderId = orderExists.OrderId,
                    productId = product.Productid,
                    Quantity = model.Quantity,
                    TotalPrice = model.Quantity * product.Price
                };
                _context.OrderDetails.Add(orderdetails);
                await _context.SaveChangesAsync();
                var cartcount = _context.OrderDetails.Where(o => o.OrderId == orderExists.OrderId).Sum(o=>o.Quantity);
                HttpContext.Session.SetInt32("CartCount", cartcount);
                return Json(new { Message = "success", CartCount=cartcount });
            }
            return Json(new { Message = "error" });

            
        }
        public async Task<IActionResult> orderdetails()
        {
            var userid = _usermanager.GetUserId(User);
            if (string.IsNullOrEmpty(userid))
            {
                return Unauthorized();
            }
            var orders = _context.Orders
                .Where(o => o.UserId == userid && o.OrderPaid == false)
                .FirstOrDefault();
            if (orders == null)
            {
                return PartialView("_CartDetailsPartial", new List<OrderDetails>());
            }
            var orderdetails = _context.OrderDetails
                .Where(od => od.OrderId == orders.OrderId)
                .Include(od => od.Product)
                .ToList();
            orders.TotalPrice = orderdetails.Sum(od => od.TotalPrice);
            await _context.SaveChangesAsync();
            return PartialView("_CartDetailsPartial", orderdetails);
        }
        [HttpPost]
        public async Task<IActionResult> remove(int orderDetailsId)
        {
            var orderdetails = await _context.OrderDetails.FindAsync(orderDetailsId);
            if (orderdetails == null)
            {
                return Json(new { Message = "error" });
            }
            _context.OrderDetails.Remove(orderdetails);
            await _context.SaveChangesAsync();
            var userid = _usermanager.GetUserId(User);
            var order = _context.Orders
                .FirstOrDefault(o => o.UserId == userid && o.OrderPaid == false);
            var cartcount = _context.OrderDetails.Where(o => o.OrderId == order.OrderId).Sum(o => o.Quantity);
            HttpContext.Session.SetInt32("CartCount", cartcount);
            return Json(new { Message = "success", CartCount = cartcount });
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userid = _usermanager.GetUserId(User);
            if (string.IsNullOrEmpty(userid))
            {
                return Unauthorized();
            }

            // Get all orders for the user ordered by date descending
            var orders = await _context.Orders
                .Where(o => o.UserId == userid)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var viewModel = new MyOrdersPageViewModel();

            foreach (var order in orders)
            {
                // Get order details with products
                var orderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == order.OrderId)
                    .Include(od => od.Product)
                        .ThenInclude(p => p.User)
                    .ToListAsync();

                // Get payment method from transaction if exists
                var transaction = await _context.Transactions
                    .Where(t => t.OrderId == order.OrderId)
                    .Include(t => t.PaymentMethod)
                    .FirstOrDefaultAsync();

                // Group items by ProductType and combine quantities
                var groupedOrderDetails = orderDetails
                    .GroupBy(od => od.Product?.Type ?? "Unknown")
                    .Select(g => new GroupedItemViewModel
                    {
                        ProductType = g.Key,
                        Quantity = g.Sum(x => x.Quantity),
                        TotalPrice = g.Sum(x => x.TotalPrice)
                    })
                    .ToList();

                var orderViewModel = new MyOrderViewModel
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    OrderPaid = order.OrderPaid,
                    TotalPrice = order.TotalPrice,
                    PaymentMethodType = transaction?.PaymentMethod?.MethodType,
                    PaymentLast4Digits = transaction?.PaymentMethod?.last4Digits,
                    OrderDetails = orderDetails.Select(od => new OrderDetailViewModel
                    {
                        Id = od.Id,
                        ProductId = od.productId,
                        ProductType = od.Product?.Type,
                        ProductDescription = od.Product?.Disc,
                        ProductPhoto = od.Product?.Photo,
                        Quantity = od.Quantity,
                        UnitPrice = od.Product?.Price ?? 0,
                        TotalPrice = od.TotalPrice,
                        ShelterName = od.Product?.User?.UserName,
                        ShelterUserId = od.Product?.User?.Id,
                        ShelterEmail = od.Product?.User?.Email,
                        ShelterPhone = od.Product?.User?.PhoneNumber
                    }).ToList(),
                    GroupedOrderDetails = groupedOrderDetails
                };

                viewModel.Orders.Add(orderViewModel);
            }

            return View(viewModel);
        }
    }
}
