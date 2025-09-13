using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Public" });
        }
    }
}