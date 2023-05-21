using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazerFinal.DataAccessLayer;
using RazerFinal.Enums;
using RazerFinal.Helpers;
using RazerFinal.Models;
using System.Data;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Order> query = _context.Orders
                .OrderByDescending(c => c.Id);

            return View(PageNatedList<Order>.Create(query, pageIndex, 5, 5));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.Orders.AnyAsync(o => o.Id == id)) return NotFound();

            Order order = await _context.Orders.Include(o => o.OrderItems.Where(oi => oi.isDeleted == false)).FirstOrDefaultAsync(o => o.Id == id && o.isDeleted == false);
            if (order == null) return NotFound();

            return View(order);

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.Orders.AnyAsync(o => o.Id == id)) return NotFound();

            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();


            IEnumerable<OrderType> orderTypes = Enum.GetValues(typeof(OrderType)).Cast<OrderType>();

            ViewBag.Statuses = orderTypes;



            return View(order);

        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int? id, Order order)
        {
            if (id == null) return BadRequest();
            if (!await _context.Orders.AnyAsync(o => o.Id == id)) return NotFound();

            Order dborder = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            IEnumerable<OrderType> orderTypes = Enum.GetValues(typeof(OrderType)).Cast<OrderType>();

            ViewBag.Statuses = orderTypes;

            dborder.Status = order.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
