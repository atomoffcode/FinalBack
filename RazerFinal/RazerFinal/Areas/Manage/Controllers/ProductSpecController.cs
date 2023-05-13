using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.ProductViewModel;
using RazerFinal.DataAccessLayer;
using RazerFinal.Helpers;
using RazerFinal.Models;
using Newtonsoft.Json;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ProductSpecController : Controller
    {
        private readonly AppDbContext _context;
        public ProductSpecController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<ProductSpec> query = _context.ProductSpecs
                .Include(s => s.Product)
                .Include(s => s.CategorySpec)
                .Include(s => s.Specification)
                .Where(s => !s.isDeleted)
                .OrderByDescending(c => c.Id);





            return View(PageNatedList<ProductSpec>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> GetCats(int productId)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            IEnumerable<CategorySpec> categorySpecs = await _context.CategorySpecs.Where(c => c.CategoryId == product.CategoryId && c.isDeleted == false).ToListAsync();

            string html = "";
            string option = "";
            foreach (CategorySpec categorySpec in categorySpecs)
            {
                option = "<option value=" + '"' + categorySpec.Id + '"' + $">{categorySpec.Name}</option>";
                html = html + option;
            }

            return Content(html);
        }
        public async Task<IActionResult> GetSpecs(int catspecId)
        {
            CategorySpec categorySpec = await _context.CategorySpecs.FirstOrDefaultAsync(p => p.Id == catspecId);
            IEnumerable<Specification> specifications = await _context.Specifications.Where(c => c.CategorySpecId == categorySpec.Id && c.isDeleted == false).ToListAsync();
            string html = "";
            string option = "";
            foreach (Specification specification in specifications)
            {
                option = "<option value=" + '"' + specification.Id + '"' + $">{specification.Name}</option>";
                html = html + option;
            }

            return Content(html);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProductVM productVM = new ProductVM
            {
                Products = await _context.Products.Where(p => p.isDeleted == false).ToListAsync(),
                CategorySpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false).ToListAsync(),
                Specifications = await _context.Specifications.Where(c => c.isDeleted == false).ToListAsync()

            };




            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            productVM.Products  = await _context.Products.Where(p => p.isDeleted == false).ToListAsync();
            productVM.CategorySpecs = await _context.CategorySpecs.Where(p => p.isDeleted == false).ToListAsync();
            productVM.Specifications = await _context.Specifications.Where(p => p.isDeleted == false).ToListAsync();




            if (productVM.Spec?.ProductId == null || !await _context.Products.AnyAsync(p => p.Id == productVM.Spec.ProductId))
            {
                return NotFound();
            }
            if (productVM.Spec?.CategorySpecId == null || !await _context.CategorySpecs.AnyAsync(p => p.Id == productVM.Spec.CategorySpecId))
            {
                return NotFound();
            }
            if (productVM.Spec?.SpecificationId == null || !await _context.Specifications.AnyAsync(p => p.Id == productVM.Spec.SpecificationId))
            {
                return NotFound();
            }
            CategorySpec categorySpec = await _context.CategorySpecs.FirstOrDefaultAsync(c => c.Id == productVM.Spec.CategorySpecId);
            Product product = await _context.Products.FirstOrDefaultAsync(c => c.Id == productVM.Spec.ProductId);
            Specification specification = await _context.Specifications.FirstOrDefaultAsync(c => c.Id == productVM.Spec.SpecificationId);


            if (product.CategoryId != categorySpec.CategoryId)
            {
                ModelState.AddModelError("Spec.CategorySpecId", $"{categorySpec.Name} adli Spesifikasiya novu bu productda yoxdur");
                return View(productVM);
            }
            if (categorySpec.Id != specification.CategorySpecId)
            {
                ModelState.AddModelError("Spec.SpecificationId", $"{categorySpec.Name} adli Spesifikasiya movcud deyil");
                return View(productVM);
            }

            if (await _context.ProductSpecs.AnyAsync(s => s.ProductId == productVM.Spec.ProductId && s.CategorySpecId == productVM.Spec.CategorySpecId && s.SpecificationId == productVM.Spec.SpecificationId && s.isDeleted == false))
            {
                ModelState.AddModelError("Spec.ProductId", $"{product.Title} - {categorySpec.Name} - {specification.Name} artiq movcuddur");
                return View(productVM);
            }

            ProductSpec productSpec = new ProductSpec
            {
                ProductId = productVM.Spec.ProductId,
                CategorySpecId = productVM.Spec.CategorySpecId,
                SpecificationId = productVM.Spec.SpecificationId,
            };

            productSpec.CreatedAt = DateTime.UtcNow.AddHours(4);
            productSpec.CreatedBy = "System";

            await _context.ProductSpecs.AddAsync(productSpec);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.ProductSpecs.AnyAsync(s=>s.Id == id && s.isDeleted == false))
            {
                return NotFound();
            }
            if (await _context.ProductSpecs.FirstOrDefaultAsync(s=>s.Id == id) == null)
            {
                return NotFound();
            }

            ProductVM productVM = new ProductVM
            {
                Spec = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.Id == id),
                Products = await _context.Products.Where(p => p.isDeleted == false).ToListAsync(),
                CategorySpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false).ToListAsync(),
                Specifications = await _context.Specifications.Where(c => c.isDeleted == false).ToListAsync()

            };




            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ProductVM productVM)
        {

            if (id == null) return BadRequest();
            if (!await _context.ProductSpecs.AnyAsync(s => s.Id == id && s.isDeleted == false))
            {
                return NotFound();
            }
            if (await _context.ProductSpecs.FirstOrDefaultAsync(s => s.Id == id) == null)
            {
                return NotFound();
            }

            ProductSpec dbps = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.Id == id);

            productVM.Products = await _context.Products.Where(p => p.isDeleted == false).ToListAsync();
            productVM.CategorySpecs = await _context.CategorySpecs.Where(p => p.isDeleted == false).ToListAsync();
            productVM.Specifications = await _context.Specifications.Where(p => p.isDeleted == false).ToListAsync();




            if (productVM.Spec?.ProductId == null || !await _context.Products.AnyAsync(p => p.Id == productVM.Spec.ProductId))
            {
                return NotFound();
            }
            if (productVM.Spec?.CategorySpecId == null || !await _context.CategorySpecs.AnyAsync(p => p.Id == productVM.Spec.CategorySpecId))
            {
                return NotFound();
            }
            if (productVM.Spec?.SpecificationId == null || !await _context.Specifications.AnyAsync(p => p.Id == productVM.Spec.SpecificationId))
            {
                return NotFound();
            }
            CategorySpec categorySpec = await _context.CategorySpecs.FirstOrDefaultAsync(c => c.Id == productVM.Spec.CategorySpecId);
            Product product = await _context.Products.FirstOrDefaultAsync(c => c.Id == productVM.Spec.ProductId);
            Specification specification = await _context.Specifications.FirstOrDefaultAsync(c => c.Id == productVM.Spec.SpecificationId);


            if (product.CategoryId != categorySpec.CategoryId)
            {
                ModelState.AddModelError("Spec.CategorySpecId", $"{categorySpec.Name} adli Spesifikasiya novu bu productda yoxdur");
                return View(productVM);
            }
            if (categorySpec.Id != specification.CategorySpecId)
            {
                ModelState.AddModelError("Spec.SpecificationId", $"{categorySpec.Name} adli Spesifikasiya movcud deyil");
                return View(productVM);
            }

            if (await _context.ProductSpecs.AnyAsync(s => s.ProductId == productVM.Spec.ProductId && s.CategorySpecId == productVM.Spec.CategorySpecId && s.SpecificationId == productVM.Spec.SpecificationId && s.isDeleted == false))
            {
                ModelState.AddModelError("Spec.ProductId", $"{product.Title} - {categorySpec.Name} - {specification.Name} artiq movcuddur");
                return View(productVM);
            }

            

            dbps.ProductId = productVM.Spec.ProductId;
            dbps.CategorySpecId = productVM.Spec.CategorySpecId;
            dbps.SpecificationId = productVM.Spec.SpecificationId;

            dbps.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbps.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.ProductSpecs.AnyAsync(p => p.Id == id)) return NotFound();
            ProductSpec productSpec = await _context.ProductSpecs.FirstOrDefaultAsync(p => p.Id == id);
            if (productSpec == null) return NotFound();


            productSpec.isDeleted = true;
            productSpec.DeletedBy = "System";
            productSpec.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();



            IQueryable<ProductSpec> query = _context.ProductSpecs
               .Include(s => s.Product)
               .Include(s => s.CategorySpec)
               .Include(s => s.Specification)
               .Where(s=>!s.isDeleted)
               .OrderByDescending(c => c.Id);

            int pageIndex;
            return PartialView("_ProductSpecIndexPartial", PageNatedList<ProductSpec>.Create(query, pageIndex=1, 3, 8));
        }
    }
}
