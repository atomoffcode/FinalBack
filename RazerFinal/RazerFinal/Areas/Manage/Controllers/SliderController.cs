using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.DataAccessLayer;
using RazerFinal.Extensions;
using RazerFinal.Helpers;
using RazerFinal.Models;
using System.Data;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Slider> query = _context.Sliders
                .Where(c => c.isDeleted == false)
            .OrderByDescending(c => c.Id);





            return View(PageNatedList<Slider>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (slider == null) return NotFound();


            return View(slider);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.IndexPosts.AnyAsync(c => c.isDeleted == false && c.MainHead.ToLower() == slider.MainHead.Trim().ToLower()))
            {
                ModelState.AddModelError("MainHead", $"{slider.MainHead} named post already exist!");
                return View(slider);
            }


            if (slider.File != null)
            {
                if (slider.File?.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "File format is not right, file must be JPEG/JPG format!");
                    return View(slider);
                }
                if ((slider.File?.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "File size is to much, must be max 300Kb!");
                }
                slider.Image = await slider.File.CreateFileAsync(_env, "assets", "photos", "category");
            }

            




            slider.MainHead = slider.MainHead.Trim();
            slider.SubHead = slider.SubHead.Trim();
            slider.CreatedAt = DateTime.UtcNow.AddHours(4);
            slider.CreatedBy = "System";

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (slider == null) return NotFound();


            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Slider slider)
        {


            if (!ModelState.IsValid)
            {
                return View(slider);
            }
            if (id == null) return BadRequest();
            if (id != slider.Id) return BadRequest();

            Slider dbslider = await _context.Sliders.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (slider == null) return NotFound();

            if (await _context.IndexPosts.AnyAsync(c => c.isDeleted == false && c.MainHead.ToLower() == slider.MainHead.Trim().ToLower() && c.Id != slider.Id))
            {
                ModelState.AddModelError("MainHead", $"{slider.MainHead} add categoryartiq movcuddur!");
                return View(slider);
            }




            if (slider.File != null)
            {
                if (slider.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "File format is not right, file must be JPEG/JPG format!");
                    return View();
                }
                if (slider.File.CheckFileLenght(300))
                {
                    ModelState.AddModelError("File", "File size is to much, must be max 300Kb!");
                    return View();
                }

                if (!string.IsNullOrWhiteSpace(dbslider.Image))
                {
                    FileHelper.DeleteFile(dbslider.Image, _env, "assets", "photos", "sliders");
                }

                dbslider.Image = await slider.File.CreateFileAsync(_env, "assets", "photos", "sliders");
            }

            dbslider.MainHead = slider.MainHead.Trim();
            dbslider.SubHead = slider.SubHead.Trim();

            dbslider.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbslider.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (slider == null) return NotFound();





            if (!string.IsNullOrWhiteSpace(slider.Image))
            {
                FileHelper.DeleteFile(slider.Image, _env, "assets", "photos", "sliders");

            }


            slider.isDeleted = true;

            slider.DeletedBy = "System";
            slider.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<IndexPost> query = _context.IndexPosts
                .Include(c => c.Product)
                .Where(c => c.isDeleted == false)
                .OrderByDescending(c => c.Id);

            int pageIndex;

            return PartialView("_IndexPostIndexPartial", PageNatedList<IndexPost>.Create(query, pageIndex = 1, 3, 8));
        }
    }
}
