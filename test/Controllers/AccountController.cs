using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Threading.Tasks;
using test.Data;
using test.Interfaces;
using test.Models;
using test.ViewModels;
using test.Repository;
using test.Services;

namespace test.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccounts _accountRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly DepiContext _context;
        private readonly PhotoServices _photoServices;


        public AccountController(IAccounts accountRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, DepiContext context, PhotoServices photoServices)
        {
            _accountRepository = accountRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            _context = context;
            _photoServices = photoServices;
        }
        public async Task<IActionResult> login(string ?ReturnUrl)
        {
            
            LoginViewModel loginViewModel = new LoginViewModel
            {
                returnUrl = ReturnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(loginViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> login(LoginViewModel user,string ?ReturnUrl)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
            user.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            var user1 = await userManager.FindByEmailAsync(user.email);
            if(user1 == null)
            {
                ModelState.AddModelError(string.Empty, "invalid attempt");
                return View(user);
            }
            if (user1 != null && !user1.EmailConfirmed && await userManager.CheckPasswordAsync(user1, user.password))
            {
                ModelState.AddModelError(string.Empty, "email not confirmed yet");
                return View(user);
            }
            var result = await signInManager.PasswordSignInAsync(user1.UserName, user.password, true, false);
            if (result.Succeeded && User.IsInRole("User"))
            {
                // Set Session Variables
                var orderExists = await _context.Orders.FirstOrDefaultAsync(o => o.UserId == user1.Id && o.OrderPaid == false);
                if (orderExists != null)
                {
                    var cartcount = _context.OrderDetails.Where(o => o.OrderId == orderExists.OrderId).Sum(o => o.Quantity);
                    HttpContext.Session.SetInt32("CartCount", cartcount);
                }
                else
                {
                    HttpContext.Session.SetInt32("CartCount", 0);
                }

                var notificationCount = await _context.ChatMessages
                    .Where(m => m.ReceiverId == user1.Id && m.read == false)
                    .Select(m => m.SenderId)
                    .Distinct()
                    .CountAsync();
                
                HttpContext.Session.SetInt32("NotificationCount", notificationCount);

                return LocalRedirect(ReturnUrl);
            }
            else if (result.Succeeded && User.IsInRole("Shelter"))
            {
                var notificationCount = await _context.ChatMessages
                    .Where(m => m.ReceiverId == user1.Id && m.read == false)
                    .Select(m => m.SenderId)
                    .Distinct()
                    .CountAsync();

                HttpContext.Session.SetInt32("NotificationCount", notificationCount);
                return LocalRedirect(ReturnUrl);
            }
            else if (result.Succeeded && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");

            }
            else
            {
                               ModelState.AddModelError(string.Empty, "invalid login attempt");
                return View(user);
            }
        }



        public async Task<IActionResult> logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login");

        }
        public IActionResult register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> register(registerviewmodel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            
            string? photoUrl = null;
            if (user.Photo != null && user.Photo.Length > 0)
            {
                var uploadResult = await _photoServices.AddPhotoAsync(user.Photo);
                if (uploadResult.Error == null)
                {
                    photoUrl = uploadResult.SecureUrl.ToString();
                }
            }
            
            var userr = new ApplicationUser { 
                UserName = user.username, 
                Email = user.email, 
                PhotoUrl = photoUrl,
                FullName = user.FullName,
                location = user.Location
            };
            var result = await userManager.CreateAsync(userr, user.password);

            if (result.Succeeded)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(userr);
                userr.PhoneNumber = user.phonenumber;
                var confirmationlink = Url.Action("ConfirmEmail", "Account", new { Userid = userr.Id, token = token }, Request.Scheme);
                string emailBody = $"Please click here to confirm your email: <a href='{confirmationlink}'>Confirm Email</a>";
                await emailSender.SendEmailAsync(userr.Email, "Email Confirmation", emailBody);
                await userManager.AddToRoleAsync(userr, user.role);
                ModelState.AddModelError(string.Empty, "Please confirm your email");
                return RedirectToAction("login");
            }
            else
            {
                foreach (var errors in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, errors.Description);
                }
                return View(user);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("NotFound");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            return View("NotFound");

        }
        [HttpPost]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);

        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string RemoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel model = new LoginViewModel
            {
                returnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (RemoteError != null)
            {
                ModelState.AddModelError(string.Empty, RemoteError);
                return View("login", model);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("login", model);
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                var user1 = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var orderExists = await _context.Orders.FirstOrDefaultAsync(o => o.UserId ==user1.Id  && o.OrderPaid == false);
                if (orderExists != null)
                {
                    var cartcount = _context.OrderDetails.Where(o => o.OrderId == orderExists.OrderId).Sum(o => o.Quantity);
                    HttpContext.Session.SetInt32("CartCount", cartcount);
                }
                else
                {
                    HttpContext.Session.SetInt32("CartCount", 0);
                }

                var notificationCount = await _context.ChatMessages
                    .Where(m => m.ReceiverId == user1.Id && m.read == false)
                    .Select(m => m.SenderId)
                    .Distinct()
                    .CountAsync();

                HttpContext.Session.SetInt32("NotificationCount", notificationCount);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Email not confirmed.");
                return View("login", model);
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        if (!user.EmailConfirmed)
                        {
                            ModelState.AddModelError(string.Empty, "Email not confirmed.");
                            return View("login", model);
                        }

                        var addLoginResult = await userManager.AddLoginAsync(user, info);
                        if (addLoginResult.Succeeded)
                        {
                            var orderExists = await _context.Orders.FirstOrDefaultAsync(o => o.UserId == user.Id && o.OrderPaid == false);
                            if (orderExists != null)
                            {
                                var cartcount = _context.OrderDetails.Where(o => o.OrderId == orderExists.OrderId).Sum(o => o.Quantity);
                                HttpContext.Session.SetInt32("CartCount", cartcount);
                            }
                            else
                            {
                                HttpContext.Session.SetInt32("CartCount", 0);
                            }

                            var notificationCount = await _context.ChatMessages
                                .Where(m => m.ReceiverId == user.Id && m.read == false)
                                .Select(m => m.SenderId)
                                .Distinct()
                                .CountAsync();

                            HttpContext.Session.SetInt32("NotificationCount", notificationCount);
                            await signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                }

                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    ModelState.AddModelError(string.Empty, "Error loading external login information during confirmation.");
                    ModelState.AddModelError(string.Empty, "Error loading external login information during confirmation.");
                    LoginViewModel loginViewModel = new LoginViewModel
                    {
                        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                    };
                    return View("login", loginViewModel);
                }

                var pictureUrl = info.Principal.FindFirstValue("urn:google:picture");
                var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
                var user = new ApplicationUser { 
                    UserName = model.Username, 
                    Email = model.Email, 
                    PhoneNumber = model.PhoneNumber, 
                    PhotoUrl = pictureUrl,
                    FullName = fullName,
                    location = model.Location 
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, model.Role);

                        // Send email confirmation
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { Userid = user.Id, token = token }, Request.Scheme);
                        string emailBody = $"Please click here to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>";
                        await emailSender.SendEmailAsync(user.Email, "Email Confirmation", emailBody);

                        ModelState.AddModelError(string.Empty, "Registration successful. Please check your email to confirm your account.");
                        LoginViewModel loginViewModel = new LoginViewModel
                        {
                            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                        };
                        return View("login", loginViewModel);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPasswordConfirmation(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user =await userManager.FindByEmailAsync(model.email);
                if (user == null|| !await userManager.IsEmailConfirmedAsync(user)) {
                    return View();
                }
                else
                {
                    var token =await userManager.GeneratePasswordResetTokenAsync(user);
                    var resetpasswordlink = Url.Action("ResetPassword","Account",new { token =token, email=model.email},Request.Scheme);
                    string emailBody = $"Please click here to reset your password: <a href='{resetpasswordlink}'>Reset Password</a>";
                    await emailSender.SendEmailAsync(model.email,"Reset Password", emailBody);
                    return View();
                }
            }
            return View("ForgetPassword",model);
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token,string email)
        {
            if(token == null || email == null)
            {
                LoginViewModel loginViewModel = new LoginViewModel
                {
                    ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                };
                return View("login", loginViewModel);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.email);
                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    var result = await userManager.ResetPasswordAsync(user, model.token, model.password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
            }
            return View(model);
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
