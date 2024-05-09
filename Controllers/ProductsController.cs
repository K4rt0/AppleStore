using Microsoft.AspNetCore.Mvc;

namespace AppleStore.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
