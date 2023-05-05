using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RazerFinal.DataAccessLayer;
using RazerFinal.Helpers;
using RazerFinal.Models;
using RazerFinal.ViewModels.FilterViewModels;
using RazerFinal.ViewModels.StoreViewModels;
using System.Drawing;

namespace RazerFinal.Controllers
{
    public class StoreController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StoreController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Shop(int catId = 0)
        {

            
            ViewBag.DefCat = "";
            List<Product> products = null;
            StoreVM storeVM = new StoreVM();
            IEnumerable<Category> categories = await _context.Categories.Include(c => c.Products.Where(p => p.isDeleted == false)).Where(c => c.isDeleted == false).ToListAsync();

            

            int firstcat = categories.MinBy(c => c.Id).Id;
            string firstcatname = categories.MinBy(c => c.Id).Name;
            string cookie = JsonConvert.SerializeObject(firstcatname);
            HttpContext.Response.Cookies.Append("cat", cookie);
            IEnumerable<CategorySpec> categorySpecs = null;
            if (catId == null)
            {
                return BadRequest();
            }
            if (catId != 0)
            {
                if (!await _context.Categories.AnyAsync(c => c.Id == catId))
                {
                    return NotFound();
                }
                else
                {
                     firstcat = categories.FirstOrDefault(c => c.Id == catId).Id;
                     firstcatname = categories.FirstOrDefault(c => c.Id == catId).Name;
                     cookie = JsonConvert.SerializeObject(firstcatname);
                    HttpContext.Response.Cookies.Append("cat", cookie);
                    ViewBag.DefCat = firstcatname;
                }
            }


            products = await _context.Products
                .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                .ThenInclude(s => s.Specification)
                .ThenInclude(s => s.CategorySpec)
                .Where(p => p.isDeleted == false && p.CategoryId == firstcat).ToListAsync();

            categorySpecs = await _context.CategorySpecs.Include(cs => cs.Specifications.Where(s => s.isDeleted == false)).Where(cs => !cs.isDeleted).Where(cs => cs.isDeleted == false && cs.CategoryId == firstcat).ToListAsync();





            double maxprice = _context.Products.Max(p => p.Price);
            double minprice = _context.Products.Min(p => p.Price);



            storeVM = new StoreVM
            {

                //Products = PageNatedList<Product>.Create(products, pageIndex, 12, 7),
                Products = products,
                Categories = categories,
                SpecCategories = categorySpecs,
            };


            return View(storeVM);
        }
        public async Task<IActionResult> CategorySorting(int catId)
        {

            ViewBag.DefCat = "";
            List<Product> products = null;
            StoreVM storeVM = new StoreVM();
            IEnumerable<Category> categories = await _context.Categories.Include(c => c.Products.Where(p => p.isDeleted == false)).Where(c => c.isDeleted == false).ToListAsync(); ;
            IEnumerable<CategorySpec> categorySpecs = null;
            if (catId == null)
            {
                return BadRequest();
            }
            if (await _context.Categories.AnyAsync(c => c.Id == catId))
            {
                products = await _context.Products.Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true)).ThenInclude(s => s.Specification).ThenInclude(s => s.CategorySpec).Where(p => p.isDeleted == false && p.CategoryId == catId).ToListAsync();

                categorySpecs = await _context.CategorySpecs.Include(cs => cs.Specifications.Where(s => s.isDeleted == false)).Where(cs => !cs.isDeleted).Where(cs => cs.CategoryId == catId && cs.isDeleted == false).ToListAsync();
                storeVM = new StoreVM
                {

                    //Products = PageNatedList<Product>.Create(products, pageIndex, 12, 7),
                    Products = products,
                    Categories = categories,
                    SpecCategories = categorySpecs,
                };
                ViewBag.DefCat = categories.FirstOrDefault(c => c.Id == catId).Name;

                string cookie = JsonConvert.SerializeObject(categories.FirstOrDefault(c => c.Id == catId).Name);
                HttpContext.Response.Cookies.Append("cat", cookie);


                return PartialView("_ShopMainPartial", storeVM);
            }
            else
            {
                return NotFound();
            }


        }
        public async Task<IActionResult> Filtering(int specId)
        {
            if (specId == null) return BadRequest();

            if (!await _context.Specifications.AnyAsync(s=>s.Id == specId))
            {
                return NotFound();
            }
            List<int> specs = new List<int>();
            string cookie = HttpContext.Request.Cookies["filter"];
            if (string.IsNullOrWhiteSpace(cookie))
            {

                specs.Add(specId);



            }
            else
            {
                specs = JsonConvert.DeserializeObject<List<int>>(cookie);
                if (specs.Contains(specId))
                {
                    //int index = Array.IndexOf(specs, specId); // find the index of the number to remove
                    //if (index != -1) // check if the number exists in the array
                    //{
                    //    List<int> list = specs.ToList(); // convert the array to a List<int>
                    //    list.RemoveAt(index); // remove the number from the list
                    //    specs = list.ToArray(); // convert the list back to an array
                    //}
                    specs.Remove(specId);
                }
                else
                {
                    specs.Add(specId);
                }

            }
            
            cookie = JsonConvert.SerializeObject(specs);
            HttpContext.Response.Cookies.Append("filter", cookie);
            cookie = HttpContext.Request.Cookies["cat"];
            string catname = JsonConvert.DeserializeObject<string>(cookie);
            List<Product> dbproducts = await _context.Products
                    .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                    .ThenInclude(s => s.Specification)
                    .ThenInclude(s => s.CategorySpec)
                    .Where(p => p.isDeleted == false && p.Category.Name == catname).ToListAsync();
            List<Product> products = new List<Product>();

            if (specs != null && specs.Count() > 0)
            {


                foreach (int spec in specs)
                {
                    //products.Add(await _context.Products
                    //.Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                    //.ThenInclude(s => s.Specification)
                    //.ThenInclude(s => s.CategorySpec)
                    //.Where(p => p.isDeleted == false && p.Category.Name == catname).FirstOrDefaultAsync(p => p.Specifications.Any(s => s.SpecificationId == spec)));

                    foreach (Product product in dbproducts.Where(p => p.Specifications.Any(s => s.SpecificationId == spec)).ToList())
                    {
                        if (!products.Contains(product))
                        {
                            products.Add(product);
                        }
                    };

                }
                return PartialView("_ShopGridPartial", products);
            }
            //else
            //{

            //    products = await _context.Products
            //        .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
            //        .ThenInclude(s => s.Specification)
            //        .ThenInclude(s => s.CategorySpec)
            //        .Where(p => p.isDeleted == false && p.Category.Name == catname).ToListAsync();

            //}

            

            return PartialView("_ShopGridPartial",dbproducts);
        }
    }
}
