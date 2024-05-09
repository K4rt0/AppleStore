using AppleStore.Migrations;
using AppleStore.Models;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace AppleStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        private readonly IProductRepository _productRepository;


        public HomeController(ILogger<HomeController> logger, INotyfService notyf, IProductRepository productRepository)
        {
            _logger = logger;
            _notyf = notyf;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = (await _productRepository.GetAllAsync()).Where(p=>p.HotSeller==true).Take(8);
            return View(products);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
