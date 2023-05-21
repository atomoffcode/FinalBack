using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using RazerFinal.DataAccessLayer;
using RazerFinal.Enums;
using RazerFinal.Extensions;
using RazerFinal.Helpers;
using RazerFinal.Models;
using RazerFinal.ViewModels;
using RazerFinal.ViewModels.AccountViewModels;
using RazerFinal.ViewModels.BasketViewModels;
using System;
using System.Data;

namespace RazerFinal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly SmtpSetting _smtpSetting;
        private readonly IWebHostEnvironment _env;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context, IConfiguration config, IOptions<SmtpSetting> smtpSetting, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _config = config;
            _smtpSetting = smtpSetting.Value;
            _env = env;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            AppUser appUser = new AppUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                IsActive = true
            };



            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }




            await _userManager.AddToRoleAsync(appUser, "Member");
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                return RedirectToAction(nameof(Profile));
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            string url = Url.Action("EmailConfirm", "Account", new { id = appUser.Id, token = token }, HttpContext.Request.Scheme, HttpContext.Request.Host.ToString());

            //string templateFullPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_EmailConfirm.cshtml");
            //string templateContent = await System.IO.File.ReadAllTextAsync(templateFullPath);
            //templateContent = templateContent.Replace("{{email}}", $"{appUser.Name} {appUser.SurName}");
            //templateContent = templateContent.Replace("{{url}}", "url");

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Email Confirmation";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = $"Please Click here to Confirm your Email {url}"
            };


            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }


            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (string.IsNullOrWhiteSpace(forgotPasswordVM.Email))
            {
                ModelState.AddModelError("", "Please enter your Email address");
                return View(forgotPasswordVM);
            }
            if(!await _userManager.Users.AnyAsync(u=>u.NormalizedEmail == forgotPasswordVM.Email.ToUpperInvariant()))
            {
                ModelState.AddModelError("", "The Email you entered is not registered!!!");
                return View(forgotPasswordVM);
            }
            AppUser appUser = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "The Email you entered is not registered!!!");
                return View(forgotPasswordVM);
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            string url = Url.Action("ResetPassword", "Account", new { id = appUser.Id, token = token }, HttpContext.Request.Scheme, HttpContext.Request.Host.ToString());

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Reset Password";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = $"Please Click here to Reset your Password {url}"
            };


            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }

            return RedirectToAction(nameof(EmailSent));
        }
        [HttpGet]
        public async Task<IActionResult> EmailSent()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> PasswordSet()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string? id, string? token)
        {
            if(string.IsNullOrWhiteSpace(id)) return BadRequest();
            if(!await _userManager.Users.AnyAsync(u=>u.Id == id)) return BadRequest();
            if(string.IsNullOrWhiteSpace(token)) return BadRequest();
            
            AppUser appUser = await _userManager.Users.Include(u=>u.Tokens).FirstOrDefaultAsync(u=>u.Id == id);

            if(appUser == null) return NotFound();

            if (appUser.Tokens != null && appUser.Tokens.Count > 0)
            {
                if (appUser.Tokens.Any(t => t.Token == token)) return BadRequest();
            }


            PasswordVM passwordVM = new PasswordVM
            {
                Id = id,
                Token = token
            };

            return View(passwordVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string? id, string? token, PasswordVM passwordVM)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            if (!await _userManager.Users.AnyAsync(u => u.Id == id)) return BadRequest();
            if (string.IsNullOrWhiteSpace(token)) return BadRequest();
            AppUser appUser = await _userManager.Users.Include(u => u.Tokens).FirstOrDefaultAsync(u => u.Id == id);

            if (appUser == null) return BadRequest();

            if (appUser.Tokens != null && appUser.Tokens.Count > 0)
            {
                if (appUser.Tokens.Any(t => t.Token == token)) return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(passwordVM);
            }

            


            IdentityResult identityResult = await _userManager.ResetPasswordAsync(appUser, token, passwordVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(passwordVM);
            }

            UserToken userToken = new UserToken
            {
                User = appUser,
                Token = token,
            };

            await _context.UserTokens.AddAsync(userToken);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PasswordSet));
        }


        [HttpGet]
        public async Task<IActionResult> EmailConfirm(string? id, string? token)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            if (string.IsNullOrWhiteSpace(token)) return BadRequest();

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) return NotFound();

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!identityResult.Succeeded)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Login));
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.isDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is Incorrect!");
                return View(loginVM);
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult = null;

            if (appUser.EmailConfirmed)
            {
                signInResult = await _signInManager
                    .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);
            }
            else
            {
                ModelState.AddModelError("", "Emailinizi Tesdiqleyin");
                return View();
            }

            if (appUser.LockoutEnd > DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Hesabiniz Bloklanib!");
                return View(loginVM);
            }


            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is Incorrect!");
                return View(loginVM);
            }

            string cookie = HttpContext.Request.Cookies["basket"];

            if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
            {
                List<BasketVM> basketVMs = new List<BasketVM>();

                foreach (Basket basket in appUser.Baskets)
                {
                    BasketVM basketVM = new BasketVM
                    {
                        Id = (int)basket.ProductId,
                        Count = basket.Count
                    };

                    basketVMs.Add(basketVM);
                }

                cookie = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", cookie);
            }
            else
            {
                HttpContext.Response.Cookies.Append("basket", "");

            }

            return RedirectToAction(nameof(Profile));
        }
        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AccountChange()
        {
            

            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders.Where(o => o.isDeleted == false)).ThenInclude(o => o.OrderItems.Where(o => o.isDeleted == false)).ThenInclude(o => o.Product)
                .Include(u => u.Addresses.Where(a => a.isDeleted == false).OrderBy(a => a.IsMain))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            AccountVM accountVM = new AccountVM
            {
                ProfileImage = appUser.ProfileImage,
                UserName = appUser.UserName,
                Email = appUser.Email,
                Name = appUser.Name,
                SurName = appUser.SurName
            };

            if (accountVM == null)
            {
                return BadRequest();
            }


            return View(accountVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> ChangePfp(AccountVM accountVM)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            AccountVM oldacc = new AccountVM
            {
                ProfileImage = user.ProfileImage,
            };
            if (accountVM.ProfileImageFile != null)
            {
                if (accountVM.ProfileImageFile.CheckFileContentType("image/jpeg") && accountVM.ProfileImageFile.CheckFileContentType("image/png"))
                {
                    ModelState.AddModelError("ProfileImageFile", "Image type is not right, must be JPEG/JPG/PNG type!");
                    return RedirectToAction("AccountChange");
                }
                if (accountVM.ProfileImageFile.CheckFileLenght(3072))
                {
                    ModelState.AddModelError("ProfileImageFile", "Image size is to much, must be max 3Mb!");
                    return RedirectToAction("AccountChange");
                }

                if (!string.IsNullOrWhiteSpace(user.ProfileImage))
                {
                    FileHelper.DeleteFile(user.ProfileImage, _env, "assets", "photos", "profile");
                }

                user.ProfileImage = await accountVM.ProfileImageFile.CreateFileAsync(_env, "assets", "photos", "profile");
            }

            IdentityResult identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return RedirectToAction("AccountChange");
            }
            accountVM.ProfileImage = user.ProfileImage;
            return RedirectToAction(nameof(AccountChange));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AccountChange(AccountVM? accountVM)
        {
            

            if (accountVM == null)
            {
                return BadRequest();
            }


            if (!ModelState.IsValid) return RedirectToAction(nameof(Profile));

            AppUser oldUser = await _userManager.FindByNameAsync(User.Identity.Name);


            if (accountVM.Email != oldUser.Email)
            {
                oldUser.Email = accountVM.Email;
            }

            if (accountVM.UserName != oldUser.UserName)
            {
                oldUser.UserName = accountVM.UserName;
            }

            oldUser.Name = accountVM.Name;
            oldUser.SurName = accountVM.SurName;

            IdentityResult identityResult = await _userManager.UpdateAsync(oldUser);


            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(accountVM);
            }

            if (!string.IsNullOrWhiteSpace(accountVM.CurrentPassword))
            {
                if (!await _userManager.CheckPasswordAsync(oldUser, accountVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Please enter your old password correctly!");
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(oldUser);

                identityResult = await _userManager.ResetPasswordAsync(oldUser, token, accountVM.Password);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }
                    return View(accountVM);
                }
            }

            await _signInManager.SignInAsync(oldUser, false);


            return RedirectToAction(nameof(Profile));
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated && !User.IsInRole("Member"))
            {
                return RedirectToAction(nameof(Login));
            }
            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders.Where(o => o.isDeleted == false)).ThenInclude(o => o.OrderItems.Where(o => o.isDeleted == false)).ThenInclude(o => o.Product)
                .Include(u => u.Addresses.Where(a => a.isDeleted == false).OrderBy(a => a.IsMain))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            AccountVM accountVM = new AccountVM
            {
                ProfileImage = appUser.ProfileImage,
                UserName = appUser.UserName,
                Email = appUser.Email,
                Name = appUser.Name,
                SurName = appUser.SurName
            };

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses,
                Address = new Address(),
                AppUser = appUser,
                AccountVM = accountVM

            };


            return View(profileVM);
        }
        
        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Addresses()
        {

            var enumValues = Enum.GetValues(typeof(Countries)).Cast<Countries>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            var selectList = new SelectList(enumValues, "Value", "Text");
            ViewBag.EnumList = selectList;


            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.isDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            List<Address> addresses = appUser.Addresses;

            ProfileVM profileVM = new ProfileVM
            {
                Addresses= addresses,
                Address = new Address(),
            };

            return View(profileVM);   
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            var enumValues = Enum.GetValues(typeof(Countries)).Cast<Countries>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            var selectList = new SelectList(enumValues, "Value", "Text");
            ViewBag.EnumList = selectList;
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.isDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses,
                Address = address,
                AppUser = appUser
            };



            if (!ModelState.IsValid)
            {
                TempData["ModelError"] = "Error";
                return View("Profile", profileVM);
            }

            if (address.IsMain == false && (appUser.Addresses == null || appUser.Addresses.Count() <= 0))
            {
                address.IsMain = true;
            }
            if (address.IsMain && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(a => a.IsMain && a.isDeleted == false))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain && a.isDeleted == false).IsMain = false;
            }

            address.CreatedAt = DateTime.UtcNow.AddHours(4);
            address.CreatedBy = $"{appUser.Name} {appUser.SurName}";

            appUser.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Addresses));
        }
        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditAddress(int? id)
        {
            var enumValues = Enum.GetValues(typeof(Countries)).Cast<Countries>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            var selectList = new SelectList(enumValues, "Value", "Text");
            ViewBag.EnumList = selectList;
            if (id == null) return BadRequest();

            Address address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.isDeleted == false);

            if (address == null) return NotFound();

            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.isDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (appUser == null) return NotFound();






            return View( address);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditAddress(Address? address, int? id)
        {
            var enumValues = Enum.GetValues(typeof(Countries)).Cast<Countries>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            var selectList = new SelectList(enumValues, "Value", "Text");
            ViewBag.EnumList = selectList;


            if (address == null) return BadRequest();
            if (id == null) return BadRequest();
            Address oldaddress = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.isDeleted == false);

            if (oldaddress == null) return NotFound();
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.isDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (appUser == null) return NotFound();

            oldaddress.Country = address.Country;
            oldaddress.City = address.City;
            oldaddress.State = address.State;
            oldaddress.PostalCode = address.PostalCode;

            if (address.IsMain != oldaddress.IsMain)
            {
                if (address.IsMain && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(a => a.IsMain && a.isDeleted == false))
                {
                    appUser.Addresses.FirstOrDefault(a => a.IsMain && a.isDeleted == false).IsMain = false;
                }
                else
                {
                    address.IsMain = true;
                }
            }
            oldaddress.IsMain = address.IsMain;

            await _context.SaveChangesAsync();

            

            //return PartialView("_AddressFormPartial", profileVM);
            return RedirectToAction(nameof(Addresses));


        }
        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DeleteAddress(int? id)
        {
            if (id == null) return BadRequest();

            Address address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.isDeleted == false);

            if (address == null) return NotFound();

            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.isDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (appUser == null) return NotFound();

            if (address.IsMain && appUser.Addresses != null && appUser.Addresses.Count() > 1 && appUser.Addresses.Any(a => a.IsMain && a.isDeleted == false))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain == false && a.isDeleted == false).IsMain = true;
            }

            address.isDeleted = true;

            await _context.SaveChangesAsync();

            appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.isDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                AppUser = appUser,
                Addresses = appUser?.Addresses
            };


            return RedirectToAction(nameof(Addresses));

        }
    }
}
