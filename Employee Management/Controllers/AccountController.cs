﻿using Employee_Management.Models;
using Employee_Management.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Employee_Management.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
           await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");

        }
       [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        //[HttpGet][HttpPost]
        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
          var user=await  userManager.FindByEmailAsync(email);
            if(user==null)
            {
                double i = 10;
                return Json(true);
            }
            else
            {
                return Json($"emali {email} is already in used");
            }    
        }
            [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email,city=model.City };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //generate email confirmation token
                        var token = await userManager.CreateAsync(user, model.Password);
                        //generate email confirmation link
                        var confimationLink = Url.Action("ConfirmEmail", "Account",
                            new { useId = user.Id, otken = token }, Request.Scheme);
                        //request scheme may be http or https
                        if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                            {
                            return RedirectToAction("ListUsers", "Administation");

                        }

                        ViewBag.ErrorTitle = "Registration Successful";
                        ViewBag.ErrorMessage = "Before you can login, please confirm your" + "email by clicking the confimation link we have mailed you.";
                        //await signInManager.SignInAsync(user,isPersistent:false);
                        //return RedirectToAction("Index","home");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if(userId==null || token==null)
            {
                return RedirectToAction("index","home");
            }
            var user =await userManager.FindByIdAsync(userId);
            if(user==null)
            {
                ViewBag.ErrorMessage = $" the userid {userId} is invalid";
                return View("Not Found");
            }
            var result=await userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Email cnnot be confirmed";
            return View("Error");
        }
        // Login action that respons to get request
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }
        // Login action that respons to post request

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model,string returnurl)
        {
            if (ModelState.IsValid)
            {
                
                
                    var result = await signInManager.PasswordSignInAsync(model.Email,
                        model.Password,model.RememberMe,false);
                    if (result.Succeeded)
                    {
                    if(!string.IsNullOrEmpty(returnurl))
                    {
                        return Redirect(returnurl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "home");

                    }
                }
                    
                        ModelState.AddModelError(string.Empty,"Invalid Login Attempt");
                    
                
                
            }

            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",new { ReturnUrl=returnUrl});
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider,redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"error from external provider :{remoteError}");
                return View("Login", model);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"error loading external login infomration");
                return View("Login", model);
            }
            var SignInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            if (SignInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)

                        };
                        await userManager.AddLoginAsync(user, info);
                    }
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                ViewBag.ErrorTitle =$"Email claim not receive form:{info.LoginProvider}";
                ViewBag.ErrorMessage = $"Please contect on support team";
                return View("Error");


            }

        }
        // for forget password
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgetPassword()
        {
           
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user!=null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);
                    return View("ForgetPasswordConfirmation");
                }
                return View("ForgetPasswordConfirmation");

            }
            return View(model);

        }

        // for reset password
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token,string email)
        {
            if (token == null|| email==null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        // for reset password
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
           if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user!=null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View();
        }
    }

}
