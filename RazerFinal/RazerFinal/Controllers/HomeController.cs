using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.DataAccessLayer;
using RazerFinal.Models;

namespace RazerFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<IndexPost> posts = await _context.IndexPosts.Where(i=>!i.isDeleted).ToListAsync();
            

            return View(posts);
        }
    }
}
