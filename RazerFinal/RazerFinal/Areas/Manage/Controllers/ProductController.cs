using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.ProductViewModel;
using RazerFinal.DataAccessLayer;
using RazerFinal.Extensions;
using RazerFinal.Helpers;
using RazerFinal.Models;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Product> query = _context.Products
                .Include(s => s.Category)
                .OrderByDescending(c => c.Id);





            return View(PageNatedList<Product>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Cats = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();
            
            return View();
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Cats = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }
            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Kateqoriya Mutleq secilmelidir!");
                return View(product);
            }
            if (!await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun kateqoriya secin!");
                return View(product);
            }

            

            

            //Many to Many
            
            


            //MultiFile Upload
            

            string seria = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0, 2);
            seria = seria.ToLower();

            int code = _context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                (int)_context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            
            product.Code = code;
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.CreatedBy = "System";



            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
    }
}
