using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.RegisterViewModels;
using RazerFinal.Models;
using System.Data;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View(loginVM);
            }
            if (!await _userManager.CheckPasswordAsync(appUser,loginVM.Password))
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View(loginVM);

            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account temporary blocked, Please try again later!");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View(loginVM);
            }

            return RedirectToAction("Index","Dashboard", new {areas = "manage"});
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                UserName = appUser.UserName,
                Email = appUser.Email
            };

            return View(profileVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {

            


            AppUser oldUser = await _userManager.FindByNameAsync(User.Identity.Name);


            IdentityResult identityResult = null;

            if (!string.IsNullOrWhiteSpace(profileVM.CurrentPassword))
            {
                if (!await _userManager.CheckPasswordAsync(oldUser, profileVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Please enter your old password coorectly!");
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(oldUser);

                identityResult = await _userManager.ResetPasswordAsync(oldUser, token, profileVM.Password);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }
                    return View(profileVM);
                }
            }

            await _signInManager.SignInAsync(oldUser, false);


            return RedirectToAction(nameof(Profile));
        }
        //[HttpGet]
        //public async Task<IActionResult> Register()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterVM registerVM)
        //{
        //    if (!ModelState.IsValid) return View(registerVM);


        //    AppUser appUser = new AppUser
        //    {
        //        Name = registerVM.Name,
        //        SurName = registerVM.SurName,
        //        Email = registerVM.Email,
        //        UserName = registerVM.UserName,

        //    };

        //    if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == registerVM.UserName.Trim().ToUpperInvariant()))
        //    {
        //        ModelState.AddModelError("UserName", $"{registerVM.UserName} username already taken");
        //        return View(registerVM);
        //    }
        //    if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerVM.UserName.Trim().ToUpperInvariant()))
        //    {
        //        ModelState.AddModelError("Email", $"{registerVM.Email} email already taken");
        //        return View(registerVM);
        //    }


        //    IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.ConfirmPassword);

        //    if (!identityResult.Succeeded)
        //    {
        //        foreach (IdentityError identityError in identityResult.Errors)
        //        {
        //            ModelState.AddModelError("", identityError.Description);
        //        }

        //        return View(registerVM);
        //    }

        //    await _userManager.AddToRoleAsync(appUser, "Admin");

        //    return Ok(registerVM);
        //}

        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));


        //    return Ok("Roles Creater Successfully");
        //}
        //public async Task<IActionResult> CreateSuperAdmin()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        Name = "Super",
        //        SurName = "Admin",
        //        UserName = "SuperAdmin",
        //        Email = "kamilka@code.edu.az"
        //    };

        //    await _userManager.CreateAsync(appUser, "RazerFianlProjectSuperAdmin20030227");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Ok("SuperAdmin Created");
        //}
    }
}
