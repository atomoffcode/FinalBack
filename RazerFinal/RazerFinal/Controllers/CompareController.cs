using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RazerFinal.DataAccessLayer;
using RazerFinal.Models;
using RazerFinal.ViewModels.CompareViewModels;

namespace RazerFinal.Controllers
{
    public class CompareController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public CompareController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {

            string cookie = HttpContext.Request.Cookies["compare"];
            List<Compare> compares = null;

            List<CategorySpec> categorySpecs = null;
            if (!string.IsNullOrEmpty(cookie))
            {
                compares = JsonConvert.DeserializeObject<List<Compare>>(cookie);
                int? catid = 0;
                foreach (Compare compare in compares)
                {
                    Product product = await _context.Products
                        .Include(p=>p.Category)
                        .Include(p => p.Specifications.Where(s => s.isDeleted == false))
                        .ThenInclude(s => s.Specification)
                        .ThenInclude(s => s.CategorySpec)
                        .FirstOrDefaultAsync(p=>p.Id == compare.Id);
                    catid = product.CategoryId;

                    if (product != null)
                    {
                        compare.Product = product;
                    }

                }
                if (catid != 0)
                {
                    categorySpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false && c.CategoryId == catid).Include(c => c.Specifications).ToListAsync();
                }
            }

            CompareVM compareVM = new CompareVM
            {
                CategorySpecs = categorySpecs,
                Compares = compares
            };  

            return View(compareVM);
        }
        public async Task<IActionResult> RemoveCompare(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.Products.AnyAsync(p => p.isDeleted == false && p.Id == id)) return NotFound();

            if (id == null) return BadRequest();
            string cookie = HttpContext.Request.Cookies["compare"];
            if (cookie == null) return BadRequest();
            List<Compare> compares = null;
            List<CategorySpec> categorySpecs = null;
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                compares = JsonConvert.DeserializeObject<List<Compare>>(cookie);
                int? catid = 0;
                if (compares.Exists(p => p.Id == id))
                {
                    compares.RemoveAll(p => p.Id == id);
                }
                cookie = JsonConvert.SerializeObject(compares);
                HttpContext.Response.Cookies.Append("compare", cookie);

                foreach (Compare compare in compares)
                {
                    Product product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                        .ThenInclude(s => s.Specification)
                        .ThenInclude(s => s.CategorySpec)
                        .FirstOrDefaultAsync(p => p.Id == compare.Id);
                    if (product != null)
                    {
                        compare.Product = product;

                    }


                }
                if (catid != 0)
                {
                    categorySpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false && c.CategoryId == catid).Include(c => c.Specifications).ToListAsync();
                }


            }
            CompareVM compareVM = new CompareVM
            {
                CategorySpecs = categorySpecs,
                Compares = compares
            };
            return PartialView("_CompareIndexPartial", compareVM);
        }
        public async Task<IActionResult> CheckCompare(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.isDeleted == false && p.Id == id)) return NotFound();

            Product pr = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p=>p.Id == id);

            string cookie = HttpContext.Request.Cookies["compare"];

            List<Compare> compares = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                compares = new List<Compare>
                {
                    new Compare {Id = (int)id,CategoryId = pr.CategoryId}
                };


            }
            else
            {
                

                compares = JsonConvert.DeserializeObject<List<Compare>>(cookie);
                if (!compares.Any(p=>p.CategoryId == pr.CategoryId))
                {
                    compares = new List<Compare>
                    {
                        new Compare {Id = (int)id,CategoryId = (int)pr.CategoryId}
                    };
                }
                else if (compares.Exists(p => p.Id == id))
                {
                    compares.RemoveAll(b => b.Id == id);
                }
                else
                {
                    if (compares.Count == 4)
                    {
                        compares.RemoveAt(0);
                    }
                    compares.Add(new Compare { Id = (int)id, CategoryId = (int)pr.CategoryId });
                };

            }
            //if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            //{
            //    AppUser appUser = await _userManager.Users
            //        .Include(u => u.Compares.Where(b => b.isDeleted == false))
            //        .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            //    if (appUser.Compares != null && appUser.Compares.Count() > 0)
            //    {
            //        if (!appUser.Compares.Exists(p => p.CategoryId == pr.CategoryId))
            //        {
            //            appUser.Compares = new List<Compare>
            //            {
            //                new Compare {Id = (int)id}
            //            };
            //        }
            //        if (appUser.Compares.Exists(b => b.ProductId == id))
            //        {
            //            appUser.Compares.RemoveAll(b => b.ProductId == id);
            //        }
            //        else
            //        {
            //            if (appUser.Compares.Count() == 4)
            //            {
            //                appUser.Compares.RemoveAt(0);
            //                Compare compare = new Compare
            //                {
            //                    ProductId = id,
            //                };
            //                appUser.Compares.Add(compare);
            //            }

            //        }
            //    }
            //    else
            //    {

            //        Compare compare = new Compare
            //        {
            //            ProductId = id,
            //        };
            //        appUser.Compares.Add(compare);


            //    }

            //    await _context.SaveChangesAsync();
            //}

            cookie = JsonConvert.SerializeObject(compares);
            HttpContext.Response.Cookies.Append("compare", cookie);

            foreach (Compare compare1 in compares)
            {
                Product product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                        .ThenInclude(s => s.Specification)
                        .ThenInclude(s => s.CategorySpec)
                        .FirstOrDefaultAsync(p => p.Id == compare1.Id);
                if (product != null)
                {
                    compare1.Product = product;

                }

            }

            return PartialView("_CompareCartPartial"); 
        }
    }
}
