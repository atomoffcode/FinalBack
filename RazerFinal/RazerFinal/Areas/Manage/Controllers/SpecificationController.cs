using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.DataAccessLayer;
using RazerFinal.Helpers;
using RazerFinal.Models;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SpecificationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SpecificationController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Specification> query = _context.Specifications
                .Include(s=>s.ProductSpecs.Where(p => p.isDeleted == false))
                .Where(c=>c.isDeleted == false)
                .OrderByDescending(c => c.Id);





            return View(PageNatedList<Specification>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Specification specification = await _context.Specifications
                .Include(c => c.ProductSpecs.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (specification == null) return NotFound();


            return View(specification);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.CatSpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false).ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specification specification)
        {
            ViewBag.CatSpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Specifications.AnyAsync(c => c.isDeleted == false && c.Name.ToLower() == specification.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{specification.Name} add specification artiq movcuddur!");
                return View(specification);
            }


            if (specification.CategorySpecId == null)
            {
                ModelState.AddModelError("CategorySpecId", "CategorySpecId mutleq secilmelidir!");
                return View(specification);
            }
            if (!await _context.CategorySpecs.AnyAsync(c => c.isDeleted == false && c.Id == specification.CategorySpecId))
            {
                ModelState.AddModelError("CategorySpecId", "CategorySpecId duzgun secilmelidir!");
                return View(specification);
            }

            if (string.IsNullOrWhiteSpace(specification.Description))
            {
                specification.Description = specification.Name;
            }




            specification.Name = specification.Name.Trim();
            specification.CreatedAt = DateTime.UtcNow.AddHours(4);
            specification.CreatedBy = "System";

            await _context.Specifications.AddAsync(specification);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Specification specification = await _context.Specifications.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (specification == null) return NotFound();

            ViewBag.CatSpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false).ToListAsync();


            return View(specification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Specification specification)
        {
            ViewBag.CatSpecs = await _context.CategorySpecs.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(specification);
            }
            if (id == null) return BadRequest();
            if (id != specification.Id) return BadRequest();

            Specification dbSpec = await _context.Specifications.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);
            if (dbSpec == null) return NotFound();

            if (specification == null) return NotFound();

            if (await _context.Specifications.AnyAsync(c => c.isDeleted == false && c.Name.ToLower() == specification.Name.Trim().ToLower() && c.Id != specification.Id))
            {
                ModelState.AddModelError("Name", $"{specification.Name} add Specification artiq movcuddur!");
                return View(specification);
            }




            if (specification.CategorySpecId != dbSpec.CategorySpecId)
            {
                if (specification.CategorySpecId == null)
                {
                    ModelState.AddModelError("CategorySpecId", "CategorySpecId mutleq secilmelidir!");
                    return View(specification);
                }
                if (!await _context.CategorySpecs.AnyAsync(c => c.isDeleted == false && c.Id == specification.CategorySpecId))
                {
                    ModelState.AddModelError("CategorySpecId", "CategorySpecId duzgun secilmelidir!");
                    return View(specification);
                }

                dbSpec.CategorySpec = specification.CategorySpec;
            }
            if (string.IsNullOrWhiteSpace(specification.Description))
            {
                dbSpec.Description = specification.Description;
            }


            dbSpec.Name = specification.Name.Trim();
            dbSpec.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbSpec.UpdatedBy = "System";

            //await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            Specification specification = await _context.Specifications
                .Include(c => c.ProductSpecs.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (specification == null) return NotFound();

            if (specification.ProductSpecs != null && specification.ProductSpecs.Count() > 0)
            {
                foreach (ProductSpec productSpec in specification.ProductSpecs)
                {
                    productSpec.isDeleted = true;
                    productSpec.DeletedBy = "System";
                    productSpec.DeletedAt = DateTime.UtcNow.AddHours(4);


                }
            }




            specification.isDeleted = true;
            specification.DeletedBy = "System";
            specification.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<Specification> query = _context.Specifications
                .Include(c => c.ProductSpecs.Where(p => p.isDeleted == false))
                .Where(c => c.isDeleted == false)
                .OrderByDescending(c => c.Id);

            int pageIndex;

            return PartialView("_SpecIndexPartial", PageNatedList<Specification>.Create(query, pageIndex = 1, 3, 8));
        }
        
    }
}
