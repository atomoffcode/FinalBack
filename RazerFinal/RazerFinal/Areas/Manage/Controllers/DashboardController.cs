using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.Areas.Manage.ViewModels.DashboardViewModels;
using RazerFinal.DataAccessLayer;
using RazerFinal.Enums;
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
            List<Product> products = await _context.Products.Where(p=>!p.isDeleted && p.Count > 0).ToListAsync();
            List<Order> orders =  await _context.Orders.Where(o=>o.isDeleted == false).Include(o=>o.SingleAddress).Include(o => o.User).Include(o=>o.OrderItems).ThenInclude(o=>o.Product).ToListAsync();

            List<OrderItem> orderItems = await _context.OrderItems.Include(o=>o.Product).Where(o => !o.isDeleted).ToListAsync();

            List<Category> categories = await _context.Categories.Where(c=>c.isDeleted == false).ToListAsync();

            
            DateTime currentDate = DateTime.Now;
            DateTime oneMonthBefore = currentDate.AddMonths(-1);
            DateTime oneDayBefore = currentDate.AddDays(-1);
            DateTime twoDayBefore = currentDate.AddDays(-2);
            float? potGrow = ((100-(float)orderItems.Where(o=>o.CreatedAt > oneMonthBefore).Sum(o=>o.Count)))/ (float)orderItems.Where(o => o.CreatedAt > oneMonthBefore).Sum(o => o.Count) * 100;
            double? growth = (double)Math.Floor((decimal)orderItems.Where(o => o.CreatedAt > oneMonthBefore).Sum(o => o.Price * o.Count));

            double? revenue = (double)Math.Floor((decimal)orderItems.Sum(o => o.Price * o.Count));

            double? allprPrices = (double)Math.Floor((decimal)products.Sum(o => o.DiscountedPrice * o.Count));

            double? income = (double)Math.Floor((decimal)orderItems.Where(o => o.CreatedAt > oneDayBefore).Sum(o => o.Price * o.Count));
            double? yesincome = (double)Math.Floor((decimal)orderItems.Where(o => o.CreatedAt <= oneDayBefore && o.CreatedAt>twoDayBefore).Sum(o => o.Price * o.Count));
            float? incPerc = (float)Math.Floor((decimal)(income > yesincome ? (income/yesincome*100)-100: income / yesincome * 100));



            List<TrVM> trVMs = new();
             
            foreach (Category item in categories)
            {
                TrVM trVM = new();
                trVM.CategoryName = item.Name;
                trVM.OrderCount = orders.Where(o => o.OrderItems.Any(o=>o.Product.CategoryId == item.Id)).Count();
                trVM.TotalSale = trVM.OrderCount > 0 ? (double)Math.Floor((decimal)orderItems.Where(s => s.Product.CategoryId == item.Id).Sum(s => s.Price*s.Count)):0;
                trVMs.Add(trVM);
                
            }



            List<Countries> countries = Enum.GetValues(typeof(Countries)).Cast<Countries>().ToList();











            DashVM dashVM = new DashVM
            {
                Orders = orders,
                Transactions = trVMs,
                PotensialGrowth = potGrow,
                Growth = growth,
                Revenue = revenue,
                RevenuePerc = (float)Math.Floor((decimal)(revenue / allprPrices) * 100),
                Income =income,
                YesIncome = yesincome,
                IncomePerc = incPerc,
                Countries = countries
            };


            return View(dashVM);
        }
    }
}
