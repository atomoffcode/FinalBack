using Microsoft.AspNetCore.Mvc;

namespace RazerFinal.Areas.Manage.Controllers
{
    [Area("manage")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
