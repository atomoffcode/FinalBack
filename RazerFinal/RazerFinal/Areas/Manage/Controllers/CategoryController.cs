using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.DataAccessLayer;
using RazerFinal.Helpers;
using RazerFinal.Models;
using RazerFinal.Extensions;
using System.Data;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Category> query = _context.Categories
                .Include(c => c.Products.Where(p => p.isDeleted == false))
                .Where(c => c.isDeleted == false)
            .OrderByDescending(c => c.Id);





            return View(PageNatedList<Category>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Products.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (category == null) return NotFound();


            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {



            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{category.Name} add categoryartiq movcuddur!");
                return View(category);
            }


            if (category.File != null)
            {
                if (category.File?.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                    return View();
                }
                if ((category.File?.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "File-in olcusu 300Kb-i kece bilmez");
                }
                category.Image = await category.File.CreateFileAsync(_env, "assets", "photos", "category");
            }



           


            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (category == null) return NotFound();

            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();


            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (id == null) return BadRequest();
            if (id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (category == null) return NotFound();

            if (await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower() && c.Id != category.Id))
            {
                ModelState.AddModelError("Name", $"{category.Name} add categoryartiq movcuddur!");
                return View(category);
            }




            if (category.File != null)
            {
                if (category.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                    return View();
                }
                if (category.File.CheckFileLenght(300))
                {
                    ModelState.AddModelError("File", "File-in olcusu 300Kb-i kece bilmez");
                    return View();
                }

                if (!string.IsNullOrWhiteSpace(dbCategory.Image))
                {
                    FileHelper.DeleteFile(dbCategory.Image, _env, "assets", "photos", "category");
                }

                dbCategory.Image = await category.File.CreateFileAsync(_env, "assets", "photos", "category");
                dbCategory.Name = category.Name.Trim();
            }

            
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbCategory.UpdatedBy = "System";

            //await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Products.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (category == null) return NotFound();

            if (category.Products != null && category.Products.Count() > 0)
            {
                foreach (Product product in category.Products)
                {
                    product.CategoryId = null;
                }
            }

            if (!string.IsNullOrWhiteSpace(category.Image))
            {
                FileHelper.DeleteFile(category.Image, _env, "assets", "photos" , "category");

            }


            category.isDeleted = true;
            category.DeletedBy = "System";
            category.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<Category> query = _context.Categories
                .Include(c => c.Products.Where(p => p.isDeleted == false))
                .Where(c => c.isDeleted == false)
                .OrderByDescending(c => c.Id);

            int pageIndex;

            return PartialView("_CategoryIndexPartial", PageNatedList<Category>.Create(query, pageIndex = 1, 3, 8));
        }
    }
}
