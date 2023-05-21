using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RazerFinal.DataAccessLayer;
using RazerFinal.Interfaces;
using RazerFinal.Models;
using RazerFinal.ViewModels.BasketViewModels;

namespace RazerFinal.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly UserManager<AppUser> _userManager;


        public LayoutService(AppDbContext context, IHttpContextAccessor httpcontextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
            _userManager = userManager;
        }
        public async Task<List<BasketVM>> GetBasket()
        {
            AppUser appUser = null;
            List<Basket> baskets = null;

            if (_httpcontextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpcontextAccessor.HttpContext.User.IsInRole("Member"))
            {
                appUser = await _userManager.Users
                    .Include(u => u.Baskets.Where(b => b.isDeleted == false)).ThenInclude(b => b.Product)
                    .FirstOrDefaultAsync(u => u.UserName == _httpcontextAccessor.HttpContext.User.Identity.Name);
                baskets = appUser.Baskets;
            }



            string cookie = _httpcontextAccessor.HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                List<BasketVM> basketVMs = null;

                if (baskets != null && baskets.Count > 0)
                {
                    basketVMs = new List<BasketVM>();
                    foreach (Basket basket in baskets)
                    {
                        Product product = basket.Product;
                        if (product != null)
                        {
                            BasketVM basketVM = new BasketVM();

                            basketVM.Id = product.Id;
                            basketVM.Count = basket.Count;
                            basketVM.Title = product.Title;
                            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            basketVM.Image = product.MainImage;
                            basketVM.ExTax = product.ExTax;

                            basketVMs.Add(basketVM);
                        }

                    }
                }
                else
                {
                    basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                    foreach (BasketVM basketVM1 in basketVMs)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM1.Id);
                        if (product != null)
                        {
                            basketVM1.Title = product.Title;
                            basketVM1.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            basketVM1.Image = product.MainImage;
                            basketVM1.ExTax = product.ExTax;
                        }

                    }
                }

                return basketVMs;
            }
            return new List<BasketVM>();
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            
            
                IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

                return settings;
            
        }

        public async Task<AppUser> GetUser()
        {
            AppUser user = null;
            if (_httpcontextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpcontextAccessor.HttpContext.User.IsInRole("Member"))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == _httpcontextAccessor.HttpContext.User.Identity.Name);
            }

            return user;
        }
    }
}
