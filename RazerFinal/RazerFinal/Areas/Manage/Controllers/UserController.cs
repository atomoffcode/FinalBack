using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.AccountViewModels;
using RazerFinal.Helpers;
using RazerFinal.Models;
using System.Data;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            List<AppUser> appUsers = await _userManager.Users.Where(u => u.UserName != User.Identity.Name).ToListAsync();

            List<UserVM> userVMs = new List<UserVM>();

            foreach (AppUser user in appUsers)
            {
                //string role = (await _userManager.GetRolesAsync(user))[0];

                UserVM userVM = new UserVM
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.UserName,
                    SurName = user.SurName,
                    Role = (await _userManager.GetRolesAsync(user))[0],
                    IsActive = user.IsActive
                };

                userVMs.Add(userVM);
            }


            return View(PageNatedList<UserVM>.Create(userVMs.AsQueryable(), pageIndex, 3, 5));
        }
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) return NotFound();

            List<RoleVM> roles = await _roleManager.Roles.Where(r => r.Name != "SuperAdmin")
                .Select(x => new RoleVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            string roleName = (await _userManager.GetRolesAsync(appUser))[0];


            ChangeRoleVM changeRoleVM = new ChangeRoleVM
            {
                Id = id,
                RoleId = roles.FirstOrDefault(r => r.Name == roleName).Id
            };

            ViewBag.Roles = roles;

            return View(changeRoleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, ChangeRoleVM changeRoleVM)
        {
            List<RoleVM> roles = await _roleManager.Roles.Where(r => r.Name != "SuperAdmin")
                .Select(x => new RoleVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();


            ViewBag.Roles = roles;


            if (!ModelState.IsValid)
            {
                return View(changeRoleVM);
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            if (string.IsNullOrWhiteSpace(changeRoleVM.RoleId))
            {
                return BadRequest();
            }

            if (id != changeRoleVM.Id)
            {
                return BadRequest();
            }

            if (!await _roleManager.Roles.AnyAsync(r => r.Id == changeRoleVM.RoleId))
            {
                return NotFound();

            }


            AppUser appUser = await _userManager.FindByIdAsync(id);

            string roleName = (await _userManager.GetRolesAsync(appUser))[0];

            if (roles.FirstOrDefault(r => r.Name == roleName).Id != changeRoleVM.RoleId)
            {
                await _userManager.RemoveFromRoleAsync(appUser, roleName);
                await _userManager.AddToRoleAsync(appUser, roles.FirstOrDefault(r => r.Id == changeRoleVM.RoleId).Name);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            //if (!await _roleManager.Roles.AnyAsync(r => r.Id == id)) return NotFound();

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser.IsActive)
            {
                appUser.IsActive = false;

                await _userManager.UpdateAsync(appUser);
            }
            else
            {
                appUser.IsActive = true;
                await _userManager.UpdateAsync(appUser);

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
