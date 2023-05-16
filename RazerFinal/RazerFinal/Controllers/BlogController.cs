using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.ProductViewModel;
using RazerFinal.DataAccessLayer;
using RazerFinal.Helpers;
using RazerFinal.Models;
using RazerFinal.ViewModels.BlogViewModels;

namespace RazerFinal.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Blog> blogs = _context.Blogs.Where(b => b.isDeleted == false);

            

            return View(PageNatedList<Blog>.Create(blogs, pageIndex, 3, 5));
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.Blogs.AnyAsync(b => b.Id == id && b.isDeleted == false)) return NotFound();

            BlogVM blogVM = new BlogVM
            {
                Blog = await _context.Blogs.Include(b => b.Comments).ThenInclude(c=>c.User).FirstOrDefaultAsync(b => b.Id == id && b.isDeleted == false),
            };

            return View(blogVM);
        }
        public async Task<IActionResult> AddComment(Comment comment)
        {
            if (comment.BlogId == null)
            {
                return BadRequest();
            }

            Blog blog = await _context.Blogs
                .Include(p => p.Comments.Where(r => r.isDeleted == false)).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == comment.BlogId);

            if (blog == null) return NotFound();

            

            if (!ModelState.IsValid)
            {
                BlogVM blogVM = new BlogVM
                {
                    Blog = await _context.Blogs.Include(b => b.Comments).ThenInclude(c=>c.User).FirstOrDefaultAsync(b => b.Id == comment.BlogId && b.isDeleted == false),
                    Comment = new Comment { BlogId = comment.BlogId },
                };
                return View("Detail", blogVM);
            }

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            comment.CreatedAt = DateTime.UtcNow.AddHours(4);
            comment.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            comment.UserId = appUser.Id;



            await _context.Comments.AddAsync(comment);

            await _context.SaveChangesAsync();

            BlogVM blogVM2 = new BlogVM
            {
                Blog = await _context.Blogs.Include(b => b.Comments).ThenInclude(c => c.User).FirstOrDefaultAsync(b => b.Id == comment.BlogId && b.isDeleted == false),
                Comment = new Comment { BlogId = comment.BlogId },
            };
            return View("Detail", blogVM2);
        }
    }
}
