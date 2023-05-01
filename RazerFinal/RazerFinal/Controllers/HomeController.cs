using Microsoft.AspNetCore.Mvc;

namespace RazerFinal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
