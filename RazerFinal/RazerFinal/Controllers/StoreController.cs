using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RazerFinal.Areas.Manage.ViewModels.ProductViewModel;
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
        public async Task<IActionResult> Product(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();

            Product product = await _context.Products
                .Include(p=>p.ProductImages.Where(p=>p.isDeleted == false))
                .Include(p => p.Specifications.Where(s => s.isDeleted == false))
                .ThenInclude(s => s.Specification)
                .ThenInclude(s => s.CategorySpec)
                .Where(p => p.isDeleted == false).OrderBy(p => p.CreatedAt)
                .FirstOrDefaultAsync(p=>p.Id == id);

            if(product == null) return NotFound();

            return View(product);
        }
        public async Task<IActionResult> Index()
        {

            StoreIndexVM storeIndexVM = new StoreIndexVM();
            List<Product> products = await _context.Products.Where(p=>!p.isDeleted).ToListAsync();

            List<Category> categories = await _context.Categories.Where(c=>!c.isDeleted).ToListAsync();

            List<Product> newProducts = products.OrderByDescending(c=>c.CreatedAt).Take(6).ToList();

            List<Product> discountedProducts = products.Where(c=>c.DiscountedPrice < c.Price).Take(6).ToList();

            List<Product> exclusiveProducts = new List<Product>();

            Product product = null;
            foreach (Category item in categories)
            {
                product = products.Where(p => p.CategoryId == item.Id).MaxBy(p => p.Price);
                if (product != null) { exclusiveProducts.Add(product); }
                
            }

            List<Slider> sliders = await _context.Sliders.Where(s=>!s.isDeleted).ToListAsync();

            storeIndexVM.Sliders = sliders;
            storeIndexVM.ExlusiveProducts = exclusiveProducts;
            storeIndexVM.DiscountedProducsts = discountedProducts;
            storeIndexVM.NewProducts = newProducts;
            storeIndexVM.Categories = categories;

            return View(storeIndexVM);
        }

        public async Task<IActionResult> Shop(int catId = 0,int sortId = 0)
        {
            string cookie0 = HttpContext.Request.Cookies["compare"];
            if (!string.IsNullOrWhiteSpace(cookie0))
            {
                List<Compare> compares = null;
                compares = JsonConvert.DeserializeObject<List<Compare>>(cookie0);
                ViewBag.Compares = compares;
            }
            else
            {
                ViewBag.Compares = null;
            }
            
            ViewBag.DefCat = "";
            List<Product> products = null;
            StoreVM storeVM = new StoreVM();
            IEnumerable<Category> categories = await _context.Categories.Include(c => c.Products.Where(p => p.isDeleted == false)).Where(c => c.isDeleted == false).ToListAsync();


            string? cookiecat = HttpContext.Request.Cookies["cat"];
            int? firstcat;
            string? firstcatname;
            string? cookie;
            if (string.IsNullOrWhiteSpace(cookiecat))
            {
                 firstcat = categories.MinBy(c => c.Id).Id;
                 firstcatname = categories.MinBy(c => c.Id).Name;
                 cookie = JsonConvert.SerializeObject(firstcatname);
                HttpContext.Response.Cookies.Append("cat", cookie);
            }
            else
            {
                cookie = HttpContext.Request.Cookies["cat"];
                firstcatname = JsonConvert.DeserializeObject<string>(cookie);
                if (string.IsNullOrWhiteSpace(firstcatname)) return BadRequest();
                if (!categories.Any(c => c.Name.ToLower() == firstcatname.ToLower())) return NotFound();
                firstcat = categories.FirstOrDefault(c => c.Name.ToLower() == firstcatname.ToLower()).Id;
                ViewBag.DefCat = firstcatname;

            }
            
            IEnumerable<CategorySpec> categorySpecs = null;
            if (catId == null || sortId == null)
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

            if (sortId != 0)
            {
                if (sortId == 1)
                {
                    products = await _context.Products
                .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                .ThenInclude(s => s.Specification)
                .ThenInclude(s => s.CategorySpec)
                .Where(p => p.isDeleted == false && p.CategoryId == firstcat).OrderBy(p => p.CreatedAt).ToListAsync();
                }
                else if (sortId == 2)
                {
                    products = await _context.Products
                .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                .ThenInclude(s => s.Specification)
                .ThenInclude(s => s.CategorySpec)
                .Where(p => p.isDeleted == false && p.CategoryId == firstcat).OrderByDescending(p => p.CreatedAt).ToListAsync();
                }
                else if (sortId > 2 || sortId < 0)
                {
                    return BadRequest();
                }
            }else if(sortId == 0)
            {
                products = await _context.Products
                .Include(p => p.Specifications.Where(s => s.isDeleted == false && s.CategorySpec.IsMain == true))
                .ThenInclude(s => s.Specification)
                .ThenInclude(s => s.CategorySpec)
                .Where(p => p.isDeleted == false && p.CategoryId == firstcat).ToListAsync();
            }
            

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
