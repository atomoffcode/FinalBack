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
    [Area("Manage")]

    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Blog> query = _context.Blogs.Where(c => c.isDeleted == false)
                .OrderByDescending(c => c.Id);


            return View(PageNatedList<Blog>.Create(query, pageIndex, 3, 3));
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Blogs.AnyAsync(b => b.Id == id)) return NotFound();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return NotFound();

            return View(blog);

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Blogs.AnyAsync(b => b.Id == id)) return NotFound();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) return NotFound();

            blog.isDeleted = true;
            blog.DeletedAt = DateTime.UtcNow.AddHours(4);
            blog.DeletedBy = "System";

            await _context.SaveChangesAsync();

            IQueryable<Blog> query = _context.Blogs
                .Where(c => c.isDeleted == false)
                .OrderByDescending(c => c.Id);

            int pageIndex;

            return PartialView("_BlogIndexPartial", PageNatedList<Blog>.Create(query, pageIndex = 1, 5, 5));

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Blogs.AnyAsync(b => b.Id == id)) return NotFound();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) return NotFound();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {

            if (id == null) return BadRequest();

            if (!await _context.Blogs.AnyAsync(b => b.Id == id)) return NotFound();

            Blog dbblog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

            if (dbblog == null) return NotFound();






            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Blogs.AnyAsync(c => c.isDeleted == false && c.Title.ToLower() == blog.Title.Trim().ToLower() && c.Id != id))
            {
                ModelState.AddModelError("Title", $"{blog.Title} Adda Title Artiq movcuddur!");
                return View(blog);
            }

            if (blog.MainDescription.Length < 5)
            {
                ModelState.AddModelError("MainDescription", "Main Descriptionda en azi 50 simvoldan ibaret cumleler omalidir");
                return View();
            }


            if (blog.File != null && blog.File.Length > 0)
            {
                if (blog.File?.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                    return View();
                }
                if ((blog.File?.Length / 1024) > 10000)
                {
                    ModelState.AddModelError("File", "File-in olcusu 10Mb-i kece bilmez");
                }

                FileHelper.DeleteFile(dbblog.Image, _env, "assets", "photos", "blogs");

                dbblog.Image = await blog.File.CreateFileAsync(_env, "assets", "photos", "blogs");
            }






            dbblog.Title = blog.Title.Trim();
            dbblog.MainDescription = blog.MainDescription;
            dbblog.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbblog.UpdatedBy = "ADMIN";


            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create(Blog blog)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Blogs.AnyAsync(c => c.isDeleted == false && c.Title.ToLower() == blog.Title.Trim().ToLower()))
            {
                ModelState.AddModelError("Title", $"{blog.Title} Adda Title Artiq movcuddur!");
                return View(blog);
            }

            if (blog.MainDescription.Length < 5)
            {
                ModelState.AddModelError("MainDescription", "Main Descriptionda en azi 50 simvoldan ibaret cumleler omalidir");
                return View();
            }


            if (blog.File == null || blog.File.Length <= 0)
            {
                ModelState.AddModelError("File", "Shekil mutleq olmalidir!");
                return View();
            }


            if (blog.File?.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                return View();
            }
            if ((blog.File?.Length / 1024) > 10000)
            {
                ModelState.AddModelError("File", "File-in olcusu 10Mb-i kece bilmez");
            }



            blog.Image = await blog.File.CreateFileAsync(_env, "assets", "photos", "blogs");



            blog.Title = blog.Title.Trim();
            blog.CreatedAt = DateTime.UtcNow.AddHours(4);
            blog.CreatedBy = "ADMIN";

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

    }
}
