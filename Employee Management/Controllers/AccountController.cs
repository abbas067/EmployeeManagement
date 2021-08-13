using Employee_Management.Models;
using Employee_Management.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                            {
                            return RedirectToAction("ListUsers", "Administation");

                        }

                        await signInManager.SignInAsync(user,isPersistent:false);
                        return RedirectToAction("Index","home");
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
        // Login action that respons to get request
        [HttpGet]
        public IActionResult Login()
        {
            return View();
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
    }
}
