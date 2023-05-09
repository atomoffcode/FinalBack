﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.DataAccessLayer;
using RazerFinal.Helpers;
using RazerFinal.Models;
using System.Data;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SpecCategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SpecCategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<CategorySpec> query = _context.CategorySpecs
                .Include(c => c.Specifications.Where(p => p.isDeleted == false))
                .Where(c=>c.isDeleted == false)
                .OrderByDescending(c => c.Id);





            return View(PageNatedList<CategorySpec>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            CategorySpec categorySpec = await _context.CategorySpecs
                .Include(c => c.Specifications.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (categorySpec == null) return NotFound();


            return View(categorySpec);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategorySpec categorySpec)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.CategorySpecs.AnyAsync(c => c.isDeleted == false && c.Name.ToLower() == categorySpec.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{categorySpec.Name} add categoryartiq movcuddur!");
                return View(categorySpec);
            }

            
                if (categorySpec.CategoryId == null)
                {
                    ModelState.AddModelError("CategoryId", "CategoryId mutleq secilmelidir!");
                    return View(categorySpec);
                }
                if (!await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == categorySpec.CategoryId ))
                {
                    ModelState.AddModelError("CategoryId", "CategoryId duzgun secilmelidir!");
                    return View(categorySpec);
                }

            if (categorySpec.IsMain != true && categorySpec.IsMain != false)
            {
                categorySpec.IsMain = false;
            }
                
            


            categorySpec.Name = categorySpec.Name.Trim();
            categorySpec.CreatedAt = DateTime.UtcNow.AddHours(4);
            categorySpec.CreatedBy = "System";

            await _context.CategorySpecs.AddAsync(categorySpec);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            CategorySpec categorySpec = await _context.CategorySpecs.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (categorySpec == null) return NotFound();

            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();


            return View(categorySpec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, CategorySpec categorySpec)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(categorySpec);
            }
            if (id == null) return BadRequest();
            if (id != categorySpec.Id) return BadRequest();

            CategorySpec dbCatSpec = await _context.CategorySpecs.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (categorySpec == null) return NotFound();

            if (await _context.CategorySpecs.AnyAsync(c => c.isDeleted == false && c.Name.ToLower() == categorySpec.Name.Trim().ToLower() && c.Id != categorySpec.Id))
            {
                ModelState.AddModelError("Name", $"{categorySpec.Name} add SpecCategory-si artiq movcuddur!");
                return View(categorySpec);
            }

            

            
                if (categorySpec.CategoryId != dbCatSpec.CategoryId)
                {
                    if (categorySpec.CategoryId == null)
                    {
                        ModelState.AddModelError("CategoryId", "CategoryId mutleq secilmelidir!");
                        return View(categorySpec);
                    }
                    if (!await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == categorySpec.CategoryId))
                    {
                        ModelState.AddModelError("CategoryId", "CategoryId duzgun secilmelidir!");
                        return View(categorySpec);
                    }

                    dbCatSpec.CategoryId = categorySpec.CategoryId;
                }
            if (dbCatSpec.IsMain != categorySpec.IsMain)
            {
                dbCatSpec.IsMain = categorySpec.IsMain;
            }
            

            dbCatSpec.Name = categorySpec.Name.Trim();
            dbCatSpec.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbCatSpec.UpdatedBy = "System";

            //await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            CategorySpec categorySpec = await _context.CategorySpecs
                .Include(c => c.Specifications.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (categorySpec == null) return NotFound();

            if (categorySpec.Specifications != null && categorySpec.Specifications.Count() > 0)
            {
                foreach (Specification specification in categorySpec.Specifications)
                {
                    specification.isDeleted = true;
                    specification.DeletedBy = "System";
                    specification.DeletedAt = DateTime.UtcNow.AddHours(4);

                    
                }
            }




            categorySpec.isDeleted = true;
            categorySpec.DeletedBy = "System";
            categorySpec.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<CategorySpec> query = _context.CategorySpecs
                .Include(c => c.Specifications.Where(p => p.isDeleted == false))
                .Where(c => c.isDeleted == false)
                .OrderByDescending(c => c.Id);

            int pageIndex;

            return PartialView("_SpecCategoryIndexPartial", PageNatedList<CategorySpec>.Create(query, pageIndex = 1, 3, 8));
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteCategory(int? id)
        //{
        //    if (id == null) return BadRequest();

        //    Category category = await _context.Categories
        //        .Include(c => c.Children.Where(ch => ch.isDeleted == false))
        //        .ThenInclude(ch => ch.Products.Where(p => p.isDeleted == false))
        //        .Include(c => c.Products.Where(p => p.isDeleted == false))
        //        .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
        //    if (category == null) return NotFound();

        //    if (category.Children != null && category.Children.Count() > 0)
        //    {
        //        foreach (Category child in category.Children)
        //        {
        //            child.isDeleted = true;
        //            child.DeletedBy = "System";
        //            child.DeletedAt = DateTime.UtcNow.AddHours(4);

        //            if (child.Products != null && child.Products.Count() > 0)
        //            {
        //                foreach (Product product in child.Products)
        //                {
        //                    product.CategoryId = null;
        //                }
        //            }
        //        }
        //    }

        //    if (category.Products != null && category.Products.Count() > 0)
        //    {
        //        foreach (Product product in category.Products)
        //        {
        //            product.CategoryId = null;
        //        }
        //    }

        //    if (!string.IsNullOrWhiteSpace(category.Image))
        //    {
        //        FileHelper.DeleteFile(category.Image, _env, "assets", "image");

        //    }


        //    category.isDeleted = true;
        //    category.DeletedBy = "System";
        //    category.DeletedAt = DateTime.UtcNow.AddHours(4);

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}
    }
}
