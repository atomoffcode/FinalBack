using Microsoft.AspNetCore.Mvc;

namespace RazerFinal.Controllers
{
    public class ContactusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
