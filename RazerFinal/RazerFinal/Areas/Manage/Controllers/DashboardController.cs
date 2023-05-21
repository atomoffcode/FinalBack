using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.DashboardViewModels;
using RazerFinal.DataAccessLayer;
using RazerFinal.Models;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Order> orders =  await _context.Orders.Where(o=>o.isDeleted == false).Include(o=>o.OrderItems).ThenInclude(o=>o.Product).ToListAsync();

            List<OrderItem> orderItems = await _context.OrderItems.Include(o=>o.Product).Where(o => !o.isDeleted).ToListAsync();

            List<Category> categories = await _context.Categories.Where(c=>c.isDeleted == false).ToListAsync();

            

            List<TrVM> trVMs = new();
             
            foreach (Category item in categories)
            {
                TrVM trVM = new();
                trVM.CategoryName = item.Name;
                trVM.OrderCount = orders.Where(o => o.OrderItems.Any(o=>o.Product.CategoryId == item.Id)).Count();
                trVM.TotalSale = trVM.OrderCount > 0 ? (double)Math.Floor((decimal)orderItems.Where(s => s.Product.CategoryId == item.Id).Sum(s => s.Price)):0;
                trVMs.Add(trVM);
                
            }

            DashVM dashVM = new DashVM
            {
                Transactions = trVMs,
            };

            return View(dashVM);
        }
    }
}
