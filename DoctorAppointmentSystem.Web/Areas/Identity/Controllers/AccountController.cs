using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.ViewModels;
using DoctorAppointmentSystem.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentSystem.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController
            (UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        ////////////////////////////////////////////////////////////////////////////////////

       

        ////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public IActionResult Register()
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM newUserVM)
        {
           
            if (ModelState.IsValid)
            {
                
                ApplicationUser user = new ApplicationUser()
                {
                    UserName=newUserVM.Name,
                    Email=newUserVM.Email,
                    PhoneNumber=newUserVM.PhoneNumber,
                };
                
               IdentityResult Result = await userManager.CreateAsync(user, newUserVM.Password);
                if (Result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    if (!roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
                    {
                        await roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
                        await roleManager.CreateAsync(new IdentityRole(SD.DoctorRole));
                        await roleManager.CreateAsync(new IdentityRole(SD.UserRole));
                        await userManager.AddToRoleAsync(user, SD.AdminRole);
                    }else
                    {
                        if (newUserVM.IsDoctor)
                            await userManager.AddToRoleAsync(user, SD.DoctorRole);
                        else
                            await userManager.AddToRoleAsync(user, SD.UserRole);

                    }

                    if(await userManager.IsInRoleAsync(user,SD.AdminRole))
                    {
                        return RedirectToAction("Index", "Admin", new { area = "User" });
                    }
                    else if (newUserVM.IsDoctor)
                    {
                        return RedirectToAction("Register", "Doctor", new { area = "User" });
                    }
                    else 
                    {
                        return RedirectToAction("Register", "Patient", new { area = "User" });
                    }
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                
            }
            return View(newUserVM);
        }


        ////////////////////////////////////////////////////////////////////////////////////
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
               ApplicationUser userModel= await userManager.FindByEmailAsync(loginVM.Email);
                if (userModel != null)
                {
                    bool IsTrue=await userManager.CheckPasswordAsync(userModel,loginVM.Password);
                    if (IsTrue)
                    {
                        await signInManager.SignInAsync(userModel, loginVM.RemmberMe);

                        if (await userManager.IsInRoleAsync(userModel, SD.AdminRole))
                        {
                            return RedirectToAction("Index", "Admin", new { area = "User" });
                        }
                        else if (await userManager.IsInRoleAsync(userModel, SD.DoctorRole))
                        {
                            return RedirectToAction("Index", "Home", new { area = "User" });
                            //return RedirectToAction("Index", "Doctor", new { area = "User" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home", new { area = "User" });
                        }
                    }

                }
                ModelState.AddModelError("", "Email or Password is wrong");
            }
            return View(loginVM);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
           await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        ////////////////////////////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////////////////////////////


    }
}
