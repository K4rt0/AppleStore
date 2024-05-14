using AppleStore.Data;
using AppleStore.Models;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AppleStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        private readonly IProductRepository _productRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, INotyfService notyf, IProductRepository productRepository, ICartItemRepository cartItemRepository, ApplicationDbContext context)
        {
            _logger = logger;
            _notyf = notyf;
            _productRepository = productRepository;
            _cartItemRepository = cartItemRepository;
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            var productList = await _productRepository.GetAllAsync();
            ViewBag.ProductList = productList;
            return View(productList);
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
