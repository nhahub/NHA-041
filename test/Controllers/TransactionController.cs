using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Interfaces;
using test.Models;
using test.ViewModels;
using test.Repository;
using test.Services;


namespace test.Controllers
{
    public class TransactionController : Controller
    { private readonly ITransaction _transactionRepository;
        private readonly IOrder _orderRepository;
        private readonly DepiContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly BraintreeService _braintreeService;

        public TransactionController(ITransaction transaction, IOrder orderRepository,DepiContext depiContext,UserManager<ApplicationUser> userManager,BraintreeService braintreeService)
        {
            _transactionRepository = transaction;
            _orderRepository = orderRepository;
            _context = depiContext;
            _usermanager = userManager;
            _braintreeService = braintreeService;
        }

        [HttpGet]
        public IActionResult ProccessPayment()
        {
            var userid = _usermanager.GetUserId(User);
            var paymentmethods = _context.PaymentMethods.Where(p => p.UserId == userid).ToList();
            var order = _context.Orders.FirstOrDefault(o => o.UserId == userid && o.OrderPaid == false);
            
            if (order == null)
            {
                return View(new ProccessPaymentViewModel
                {
                    paymentMethods = paymentmethods,
                    orderDetails = new List<OrderDetails>()
                });
            }

            var orderdetails = _context.OrderDetails
                .Where(od => od.OrderId == order.OrderId)
                .Include(od => od.Product)
                .ToList();

            // Group items by product and sum quantities
            var groupedItems = orderdetails
                .GroupBy(od => od.Product?.Type ?? "Unknown")
                .Select(g => new GroupedItemViewModel
                {
                    ProductType = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    TotalPrice = g.Sum(x => x.TotalPrice)
                })
                .ToList();

            var model = new ProccessPaymentViewModel
            {
                paymentMethods = paymentmethods,
                orderid = order.OrderId,
                totalprice = order.TotalPrice,
                orderDetails = orderdetails,
                GroupedItems = groupedItems,
                TotalQuantity = orderdetails.Sum(x => x.Quantity)
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProccessPayment(ProccessPaymentViewModel model)
        {
           var order=await _orderRepository.GetOrderFortransaction(model.orderid);
            var payment= _context.PaymentMethods.Where(p => p.PaymentMethodId == model.selectedPaymentMethodid).Select(p=>new PaymentMethods
            {
                PaymentMethodId=p.PaymentMethodId,
                GatewayToken=p.GatewayToken
            }).FirstOrDefault();

            if (payment == null)
            {
                return Json(new { status = "failed", message = "Please select a valid payment method." });
            }

            var orderdetails = new List<OrderDetails>();
            orderdetails = _context.OrderDetails
                .Where(od => od.OrderId == order.OrderId)
                .Include(od => od.Product)
                .ToList();
            _transactionRepository.beginTransaction();
            try
            {
                if (order == null)
                {
                    var Message = "Order not found.";
                    return Json(new { status = "failed", message = Message });
                }
                if (order.OrderPaid == true)
                {
                    var Message = "Order is already paid.";
                    return Json(new { status = "failed", message = Message });
                }
                foreach (var item in orderdetails)
                {
                    
                    if (item.Product == null)
                    {
                        _transactionRepository.rollbackTransaction();
                        var Message = $"Product with ID {item.productId} not found.";
                        return Json(new { status = "failed", message = Message });
                    }
                    if (item.Product.Quantity < item.Quantity)
                    {
                        _transactionRepository.rollbackTransaction();
                        var Message = $"Insufficient stock for product {item.Product.Type}. Available stock: {item.Product.Quantity}, Requested quantity: {item.Quantity}.";
                        return Json(new { status = "failed", message = Message });
                    }
                }
                // Process payment with Braintree
                var paymentResult = _braintreeService.Sale(payment.GatewayToken, order.TotalPrice);

                if (!paymentResult.Success)
                {
                    _transactionRepository.rollbackTransaction();
                    return Json(new { status = "failed", message = paymentResult.ErrorMessage ?? "Payment failed." });
                }

                var transactionResult = _transactionRepository.AddTransaction(new Transactions
                {
                    OrderId = order.OrderId,
                    TransactionDate = DateTime.Now,
                    Amount = order.TotalPrice,
                    PaymentMethodId = payment.PaymentMethodId,
                    Status = "Paid"
                });

                if (transactionResult)
                {
                    order.OrderPaid = true;
                    foreach(var item in orderdetails)
                    {
                        item.Product.Quantity -= item.Quantity;
                    }

                    await _transactionRepository.savechangesAsync();
                    _transactionRepository.commitTransaction();
                    var Message = "Payment processed successfully.";
                    HttpContext.Session.SetInt32("CartCount", 0);
                    return Json(new { status = "succeeded", message = Message });
                }

                else
                {
                    _transactionRepository.rollbackTransaction();
                    var Message = "Payment processing failed.";
                    return Json(new {status="failed",message=Message});
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _transactionRepository.rollbackTransaction();
                var Message = "The item went out of stock while processing the payment";
                return Json(new { status = "failed", message = Message });
            }
            catch (Exception ex)
            {
                _transactionRepository.rollbackTransaction();
                return Json(new { status = "failed", message = ex.Message });
            }
            

        }
    }
}
