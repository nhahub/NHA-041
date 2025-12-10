using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using test.Data;
using test.Models;
using test.ViewModels;
using test.Services;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace test.Controllers
{
    public class PaymentMethodController : Controller
    {
        private readonly DepiContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BraintreeService _braintreeService;

        public PaymentMethodController(DepiContext context, UserManager<ApplicationUser> userManager,BraintreeService braintreeService)
        {
            _context = context;
            _userManager = userManager;
            _braintreeService = braintreeService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = _braintreeService.GetClientToken();
            ViewBag.ClientToken = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePaymentMethodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var result = _braintreeService.CreatePaymentMethod(model.Nonce, user.Id);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, "Error creating payment method: " + result.Error);
                    return View(model);
                }


                    var paymentMethod = new PaymentMethods
                    {
                        UserId = user.Id,
                        GatewayToken = result.Token,      
                        last4Digits = result.Last4,      
                        MethodType = result.CardType,      
                        expiryMonth = result.ExpiryMonth,  
                        expiryYear = result.ExpiryYear     
                    };

                    _context.PaymentMethods.Add(paymentMethod);
                    await _context.SaveChangesAsync();

                    // Redirect back to the payment process page
                    return RedirectToAction("ProccessPayment", "Transaction");
                }

                return View(model);
            
        }
        [HttpGet]
            public async Task<IActionResult> Edit(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
            {
                return RedirectToAction("ProccessPayment", "Transaction");
            }
            var model = new EditPaymentMethodViewModel
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                ExpiryMonth = int.Parse(paymentMethod.expiryMonth),
                ExpiryYear = int.Parse(paymentMethod.expiryYear)
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (EditPaymentMethodViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var paymentMethod = await _context.PaymentMethods.FindAsync(model.PaymentMethodId);

            if (paymentMethod == null)
            {
                ModelState.AddModelError("", "Payment method not found.");
                return (RedirectToAction("ProccessPayment", "Transaction"));
            }
            paymentMethod.expiryMonth = model.ExpiryMonth.ToString("00");
            paymentMethod.expiryYear = model.ExpiryYear.ToString();
            _context.PaymentMethods.Update(paymentMethod);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProccessPayment", "Transaction");


        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var paymentmethod =await _context.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == id);
            if (paymentmethod == null)
            {
                ModelState.AddModelError(string.Empty, "The payment method already deleted");
                return RedirectToAction("ProccessPayment", "Transaction");
            }
             _context.PaymentMethods.Remove(paymentmethod);
          await  _context.SaveChangesAsync();
            return RedirectToAction("ProccessPayment", "Transaction");
        }

    }
}
