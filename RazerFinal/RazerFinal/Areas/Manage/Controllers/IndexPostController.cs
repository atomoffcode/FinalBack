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

    public class IndexPostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public IndexPostController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<IndexPost> query = _context.IndexPosts
                .Include(c => c.Product)
                .Where(c => c.isDeleted == false)
            .OrderByDescending(c => c.Id);





            return View(PageNatedList<IndexPost>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            IndexPost indexPost = await _context.IndexPosts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (indexPost == null) return NotFound();


            return View(indexPost);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products.Where(c => c.isDeleted == false).ToListAsync();


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IndexPost indexPost)
        {
            ViewBag.Products = await _context.Products.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.IndexPosts.AnyAsync(c => c.isDeleted == false && c.MainHead.ToLower() == indexPost.MainHead.Trim().ToLower()))
            {
                ModelState.AddModelError("MainHead", $"{indexPost.MainHead} named post already exist!");
                return View(indexPost);
            }


            if (indexPost.File != null)
            {
                if (indexPost.File?.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "File format is not right, file must be JPEG/JPG format!");
                    return View(indexPost);
                }
                if ((indexPost.File?.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "File size is to much, must be max 300Kb!");
                }
                indexPost.Image = await indexPost.File.CreateFileAsync(_env, "assets", "photos", "category");
            }

            if(!await _context.Products.AnyAsync(p=>p.isDeleted == false && p.Id == indexPost.ProductId))
            {
                ModelState.AddModelError("ProductId", "Select right Product!");
                return View(indexPost);
            }




            indexPost.MainHead = indexPost.MainHead.Trim();
            indexPost.SubHead = indexPost.SubHead.Trim();
            indexPost.CreatedAt = DateTime.UtcNow.AddHours(4);
            indexPost.CreatedBy = "System";

            await _context.IndexPosts.AddAsync(indexPost);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            IndexPost indexPost = await _context.IndexPosts.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (indexPost == null) return NotFound();

            ViewBag.Products = await _context.Products.Where(c => c.isDeleted == false).ToListAsync();


            return View(indexPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, IndexPost indexPost)
        {
            ViewBag.Products = await _context.Products.Where(c => c.isDeleted == false).ToListAsync();


            if (!ModelState.IsValid)
            {
                return View(indexPost);
            }
            if (id == null) return BadRequest();
            if (id != indexPost.Id) return BadRequest();

            IndexPost dbIndexPost = await _context.IndexPosts.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (indexPost == null) return NotFound();

            if (await _context.IndexPosts.AnyAsync(c => c.isDeleted == false && c.MainHead.ToLower() == indexPost.MainHead.Trim().ToLower() && c.Id != indexPost.Id))
            {
                ModelState.AddModelError("MainHead", $"{indexPost.MainHead} add categoryartiq movcuddur!");
                return View(indexPost);
            }




            if (indexPost.File != null)
            {
                if (indexPost.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "File format is not right, file must be JPEG/JPG format!");
                    return View();
                }
                if (indexPost.File.CheckFileLenght(300))
                {
                    ModelState.AddModelError("File", "File size is to much, must be max 300Kb!");
                    return View();
                }

                if (!string.IsNullOrWhiteSpace(dbIndexPost.Image))
                {
                    FileHelper.DeleteFile(dbIndexPost.Image, _env, "assets", "photos", "sliders");
                }

                dbIndexPost.Image = await indexPost.File.CreateFileAsync(_env, "assets", "photos", "sliders");
            }

            if (!await _context.Products.AnyAsync(p => p.isDeleted == false && p.Id == indexPost.ProductId))
            {
                ModelState.AddModelError("ProductId", "Select right Product!");
                return View(indexPost);
            }

            dbIndexPost.MainHead = indexPost.MainHead.Trim();
            dbIndexPost.SubHead = indexPost.SubHead.Trim();
            dbIndexPost.ProductId = indexPost.ProductId;
            dbIndexPost.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbIndexPost.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            IndexPost indexPost = await _context.IndexPosts
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (indexPost == null) return NotFound();

            



            if (!string.IsNullOrWhiteSpace(indexPost.Image))
            {
                FileHelper.DeleteFile(indexPost.Image, _env, "assets", "photos", "sliders");

            }


            indexPost.isDeleted = true;

            indexPost.DeletedBy = "System";
            indexPost.DeletedAt = DateTime.UtcNow.AddHours(4);

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
