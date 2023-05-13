using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.ProductViewModel;
using RazerFinal.DataAccessLayer;
using RazerFinal.Extensions;
using RazerFinal.Helpers;
using RazerFinal.Models;
using System.Data;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
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
                .Where(p=>p.isDeleted == false)
                .OrderByDescending(c => c.Id);





            return View(PageNatedList<Product>.Create(query, pageIndex, 3, 8));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null) return BadRequest();
            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();
            Product product = await _context.Products
                .Include(p=>p.ProductImages.Where(p=>p.isDeleted == false))
                .Include(p=>p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if(product == null) return NotFound();
            List<ProductSpec> productSpecs = await _context.ProductSpecs
                .Include(p=>p.Specification)
                .Include(p => p.CategorySpec)
                .Where(p => p.isDeleted == false && p.ProductId == product.Id).ToListAsync();

            ProductDetailVM detailVM = new ProductDetailVM
            {
                Product = product,
                ProductSpecs = productSpecs
            };

            return View(detailVM);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Cats = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();
            
            return View();
        }
        [HttpPost]
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
                ModelState.AddModelError("CategoryId", "Choose the right category please");
                return View(product);
            }
            if (!await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Choose the right category please");
                return View(product);
            }
            if (string.IsNullOrWhiteSpace(product.Title))
            {
                ModelState.AddModelError("Title", "Please enter the product title");
                return View(product);
            }
            if (await _context.Products.AnyAsync(p=>p.Title.ToLower() == product.Title.Trim().ToLower()))
            {
                ModelState.AddModelError("Title", "There is alreay a product with this title");
                return View(product);
            }
            if (string.IsNullOrWhiteSpace(product.Description))
            {
                ModelState.AddModelError("Description", "Description should be written");
                return View(product);
            }
            if (product.Price == null)
            {
                ModelState.AddModelError("Price", "Price is necessary!");
                return View(product);
            }
            if (product.DiscountedPrice == null)
            {
                ModelState.AddModelError("DiscountedPrice", "Discounted Price is necessary!");
                return View(product);
            }
            if (product.DiscountedPrice > product.Price)
            {

                ModelState.AddModelError("DiscountedPrice", "Discounted Price can't be more than its Price");
                return View(product);
            }
            if (product.ExTax == null)
            {
                ModelState.AddModelError("ExTax", "ExTax is necessary!");
                return View(product);
            }
            if (product.Count == null)
            {
                ModelState.AddModelError("Count", "Count is necessary!");
                return View(product);
            }
            if (product.Count < 0)
            {
                ModelState.AddModelError("Count", "Count can't be less than zero");
                return View(product);
            }


            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} named file format is not right!");
                    return View(product);
                }
                if (product.MainFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} named file size is too much, the size can be max 300Kb");
                    return View(product);
                }

                product.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "photos", "products");
            }
            else
            {
                ModelState.AddModelError("MainFile", "The Main picture is necessary");
                return View(product);
            }



            if (product.Files != null & product.Files.Count() > 5)
            {
                ModelState.AddModelError("Files", "Files quantity is more than 5, they should not be more than 5!");
                return View(product);
            }


            //MultiFile Upload
            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} named file format is not right!");
                        return View(product);
                    }
                    if (file.CheckFileLenght(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} named file size is too much, the size can be max 300Kb");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Iamge = await file.CreateFileAsync(_env, "assets", "photos", "products"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);


                }
                product.ProductImages = productImages;
            }
            else
            {
                ModelState.AddModelError("Files", "Pictures of product is necessary!");
                return View(product);
            }





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
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Cats = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();
            if (id == null) return BadRequest();
            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();
            Product product = await _context.Products.Include(p=>p.ProductImages.Where(p=>p.isDeleted == false)).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Cats = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (id == null)
            {
                return BadRequest();
            }
            if (id != product.Id) return BadRequest();

            Product dbproduct = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.isDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.isDeleted == false);

            if (dbproduct == null) return NotFound();

            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Category is necessary!");
                return View(product);
            }
            if (!await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Select the right category!");
                return View(product);
            }
            
            if (string.IsNullOrWhiteSpace(product.Title))
            {
                ModelState.AddModelError("Title", "Title is necessary!");
                return View(product);
            }
            if (string.IsNullOrWhiteSpace(product.Description))
            {
                ModelState.AddModelError("Description", "Description is necessary!");
                return View(product);
            }
            if(await _context.Products.AnyAsync(p => p.Title.ToLower() == product.Title.Trim().ToLower() && p.Title.ToLower()!=dbproduct.Title.ToLower()))
            {
                ModelState.AddModelError("Title", $"{product.Title.Trim()}There is alreay a product with {product.Title.Trim()} title !");
                return View(product);
            }
            if (product.Price == null)
            {
                ModelState.AddModelError("Price", "Price is necessary!");
                return View(product);
            }
            if (product.DiscountedPrice == null)
            {
                ModelState.AddModelError("DiscountedPrice", "Discounted Price is necessary!");
                return View(product);
            }
            if (product.DiscountedPrice > product.Price)
            {

                ModelState.AddModelError("DiscountedPrice", "Discounted Price can't be more than its Price");
                return View(product);
            }
            if (product.ExTax == null)
            {
                ModelState.AddModelError("ExTax", "ExTax is necessary!");
                return View(product);
            }
            if (product.Count == null)
            {
                ModelState.AddModelError("Count", "Count is necessary!");
                return View(product);
            }
            if (product.Count < 0)
            {
                ModelState.AddModelError("Count", "Count can't be less than zero");
                return View(product);
            }
            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} named file is not right");
                    return View(product);
                }
                if (product.MainFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} named file size is more than 300Kb, max file size is 300Kb");
                    return View(product);
                }

                FileHelper.DeleteFile(dbproduct.MainImage, _env, "assets", "photos", "products");


                dbproduct.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "photos", "products");
            }
            int canUpload = 5 - (dbproduct.ProductImages != null ? dbproduct.ProductImages.Count : 0);

            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maximum {canUpload} picture you can upload");
                return View(dbproduct);
            }

            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} named file is not right");
                        return View(product);
                    }
                    if (file.CheckFileLenght(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} named file size is more than 300Kb, max file size is 300Kb");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Iamge = await file.CreateFileAsync(_env, "assets", "photos", "products"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);


                }
                dbproduct.ProductImages.AddRange(productImages);
            }

            if (dbproduct.CategoryId != product.CategoryId)
            {
                List<ProductSpec> productSpecs = await _context.ProductSpecs.Where(p => p.isDeleted == false).ToListAsync();
                foreach (ProductSpec spec in productSpecs)
                {
                    spec.isDeleted = true;
                    spec.DeletedBy = "System";
                    spec.DeletedAt = DateTime.UtcNow.AddHours(4);
                }
                await _context.SaveChangesAsync();
            }

            dbproduct.Title = product.Title;
            dbproduct.Description = product.Description;
            dbproduct.Price = product.Price;
            dbproduct.DiscountedPrice = product.DiscountedPrice;
            dbproduct.ExTax = product.ExTax;
            dbproduct.Count = product.Count;
            dbproduct.UpdatedBy = "System";
            dbproduct.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {

            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.isDeleted == false))
                .FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            if (!product.ProductImages.Any(pi => pi.Id == imageId)) return BadRequest();

            if (product.ProductImages.Count <= 1)
            {
                return BadRequest();
            }


            product.ProductImages.FirstOrDefault(p => p.Id == imageId).isDeleted = true;
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedBy = "System";
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(product.ProductImages.FirstOrDefault(p => p.Id == imageId).Iamge, _env, "assets", "photos", "products");



            return PartialView("_ProductImagePartial", product.ProductImages.Where(pi => pi.isDeleted == false).ToList());
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            if(!await _context.Products.AnyAsync(p=>p.Id == id)) return NotFound();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            product.isDeleted = true;
            List<ProductSpec> productSpecs = await _context.ProductSpecs.Where(p => p.ProductId == product.Id && p.isDeleted == false).ToListAsync();
            if (productSpecs != null && productSpecs.Count > 0)
            {
                foreach (ProductSpec spec in productSpecs)
                {
                    spec.isDeleted = true;
                    spec.DeletedBy = "System";
                    spec.DeletedAt = DateTime.UtcNow.AddHours(4);
                }
            }

            product.DeletedBy = "System";
            product.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();


            IQueryable<Product> query = _context.Products
                .Include(s => s.Category)
                .Where(p => p.isDeleted == false)
                .OrderByDescending(c => c.Id);

            int pageIndex;



            return View(PageNatedList<Product>.Create(query, pageIndex=1, 3, 8));
        }

    }
}
