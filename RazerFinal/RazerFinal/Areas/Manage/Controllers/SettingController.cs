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
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Setting> query = _context.Settings
                .OrderByDescending(c => c.Id);





            return View(PageNatedList<Setting>.Create(query, pageIndex, 5, 3));
        }




        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Setting setting = await _context.Settings.FirstOrDefaultAsync(c => c.Id == id);

            if (setting == null) return NotFound();




            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? id, Setting setting)
        {


            if (!ModelState.IsValid)
            {
                return View(setting);
            }
            if (id == null) return BadRequest();
            if (id != setting.Id) return BadRequest();

            Setting dbSetting = await _context.Settings.FirstOrDefaultAsync(c => c.Id == id);

            if (setting == null) return NotFound();

            if (await _context.Settings.AnyAsync(c => c.Key.ToLower() == setting.Key.Trim().ToLower() && c.Id != setting.Id))
            {
                ModelState.AddModelError("Key", $"{setting.Key} named Key already exist!");
                return View(setting);
            }

            if (setting.Value == null)
            {
                ModelState.AddModelError("Value", "Value must be");
                return View(setting);
            }



            if (setting.File != null)
            {
                if (setting.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "File format is not right, file must be JPEG/JPG format");
                    return View();
                }
                if (setting.File.CheckFileLenght(3000))
                {
                    ModelState.AddModelError("File", "File size is to much, File size must be max 3Mb");
                }

                if (dbSetting.File != null)
                {
                    FileHelper.DeleteFile(dbSetting.Image, _env, "assets", "photos" , "icons");
                    dbSetting.Image = await setting.File.CreateFileAsync(_env, "assets", "photos" , "icons");


                }
                else
                {

                    dbSetting.Image = await setting.File.CreateFileAsync(_env, "assets", "photos" , "icons");
                }

            }

            dbSetting.Key = setting.Key.Trim();
            dbSetting.Value = setting.Value.Trim();

            //await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
