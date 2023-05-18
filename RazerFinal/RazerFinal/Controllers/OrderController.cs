using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using RazerFinal.DataAccessLayer;
using RazerFinal.Enums;
using RazerFinal.Models;
using RazerFinal.ViewModels;
using RazerFinal.ViewModels.OrderViewModels;
using System.Data;
using System.Security.Policy;

namespace RazerFinal.Controllers
{
    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly SmtpSetting _smtpSetting;


        public OrderController(UserManager<AppUser> userManager, AppDbContext context, IOptions<SmtpSetting> smtpSetting)
        {
            _userManager = userManager;
            _context = context;
            _smtpSetting = smtpSetting.Value;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if(!User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                return RedirectToAction("Login","Account");
            }

            AppUser appUser = await _userManager.Users.Include(u=>u.Orders.Where(o=>!o.isDeleted)).ThenInclude(o=>o.OrderItems.Where(o=>!o.isDeleted)).ThenInclude(o=>o.Product).FirstOrDefaultAsync(u=>u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (appUser == null) return NotFound();

           

            List<Order> orders = appUser.Orders;
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Complete(int? id)
        {
            Order order = await _context.Orders
                .Include(o => o.OrderItems.Where(o => o.OrderId == id && o.isDeleted == false)).ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            
            




            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.isDeleted == false))
                .ThenInclude(b => b.Product)
                .Include(u => u.Addresses.Where(a => a.IsMain && a.isDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (appUser.Baskets == null || appUser.Baskets.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //if (appUser.Addresses == null || appUser.Addresses.Count <= 0)
            //{
            //    return RedirectToAction("profile", "account");
            //}

            if (appUser.Addresses != null && appUser.Addresses.Count > 0)
            {
                SelectList selectList = new SelectList(appUser.Addresses, nameof(Address.Id), nameof(Address.FullAddress));
                ViewBag.Addresses = selectList;

            }
            else
            {
                ViewBag.Addresses = null;

            }
            var enumValues = Enum.GetValues(typeof(Countries)).Cast<Countries>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            SelectList selectList2 = new SelectList(enumValues, "Value", "Text");
            ViewBag.EnumList = selectList2;

            List<Basket> baskets = new List<Basket>();

            foreach (Basket item in appUser.Baskets)
            {
                item.Price = item.Product.Price;
                item.ExTax = item.Product.ExTax;
                baskets.Add(item);
            }


            OrderVM orderVM = new OrderVM
            {
                Order = new Order
                {
                    Name = appUser.Name,
                    SurName = appUser.SurName,
                    Email = appUser.Email,
                    Country = (appUser.Addresses != null && appUser.Addresses.Count > 0 ? appUser.Addresses.First().Country: null),
                    City = (appUser.Addresses != null && appUser.Addresses.Count > 0 ? appUser.Addresses.First().City : null),
                    State = (appUser.Addresses != null && appUser.Addresses.Count > 0 ? appUser.Addresses.First().State : null),
                    PostalCode = (appUser.Addresses != null && appUser.Addresses.Count > 0 ? appUser.Addresses.First().PostalCode : null)
                },
                Baskets = baskets,
            };



            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
                 .Include(u => u.Baskets.Where(b => b.isDeleted == false)).ThenInclude(b => b.Product)
                 .Include(u => u.Addresses.Where(a => a.IsMain && a.isDeleted == false))
                 .Include(u => u.Orders)
                 .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            SelectList selectList = new SelectList(appUser.Addresses, nameof(Address.Id), nameof(Address.FullAddress));
            ViewBag.Addresses = selectList;
            var enumValues = Enum.GetValues(typeof(Countries)).Cast<Countries>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            SelectList selectList2 = new SelectList(enumValues, "Value", "Text");
            ViewBag.EnumList = selectList2;

            List<Basket> baskets = new List<Basket>();

            foreach (Basket item in appUser.Baskets)
            {
                item.Price = item.Product.Price;
                item.ExTax = item.Product.ExTax;
                baskets.Add(item);    
            }
            if (!order.CustomAddress)
            {
                Address address = appUser.Addresses.FirstOrDefault(a=>a.Id == order.SingleAddressId);
                order.Name = address.User.Name;
                order.SurName = address.User.SurName;
                order.Email = address.User.Email;
                order.PhoneNumber = address.PhoneNumber;
                order.Country = address.Country;
                order.City = address.City;
                order.State = address.State;
                order.PostalCode = address.PostalCode;
                order.DirectAddress = address.DirectAddress;
            }
            OrderVM orderVM = new OrderVM
            {
                Order = order,
                Baskets = baskets
            };

            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.No = appUser.Orders != null && appUser.Orders.Count > 0 ? appUser.Orders.Last().No + 1 : 1;
            order.Status = OrderType.Pending;
            appUser.Orders.Add(order);
            order.OrderItems = new List<OrderItem>();
            foreach (Basket basket in appUser.Baskets)
            {
                basket.isDeleted = true;

                OrderItem orderItem = new OrderItem
                {
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}",
                    Count = basket.Count,
                    ProductId = basket.ProductId,
                    Price = basket.Product.DiscountedPrice > 0 ? basket.Product.DiscountedPrice : basket.Product.Price,
                };

                order.OrderItems.Add(orderItem);
            }

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Purschase receipt";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"<ul >\r\n                                Your Receipt\r\n                                <li >No: {order.No}</li>\r\n                                <li >Date: {order.CreatedAt.ToString("dd/mm/yyyy")}</li>\r\n                                <li >Address: {order.Country} , {order.City}, {order.DirectAddress}, {order.PostalCode}</li>\r\n                                <li >Quantity{order.OrderItems.Count}</li>\r\n                                <li >Total Price:US${order.OrderItems.Sum(o => o.Price)}</li>\r\n                                <li >{order.Status.ToString()}</li>\r\n                                \r\n                            </ul>"

            };

            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }

            HttpContext.Response.Cookies.Append("basket", "");

            await _context.SaveChangesAsync();

            TempData["Error"] = $"{order.No} Sifarisiniz ugurla gonderildi";

            return RedirectToAction("Complete", new { id = order.Id });
        }
    }
}
