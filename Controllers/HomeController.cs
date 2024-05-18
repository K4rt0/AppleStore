using AppleStore.Data;
using AppleStore.Migrations;
using AppleStore.Models;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace AppleStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        private readonly ICartItemRepository _cartItemRepository;

        public HomeController(IProductRepository productRepository, 
            IProductAttributeRepository productAttributeRepository, 
            ICategoryRepository categoryRepository, 
            IProductVariantRepository productVariantRepository, 
            ApplicationDbContext context, INotyfService notyf,
            ICartItemRepository cartItemRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productVariantRepository = productVariantRepository;
            _productAttributeRepository = productAttributeRepository;
            _context = context;
            _notyf = notyf;
            _productRepository = productRepository;
            _cartItemRepository = cartItemRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = (await _productRepository.GetAllAsync()).Take(8);
            List<decimal> minPrices = new List<decimal>();
            foreach (var product in products)
            {
                var minPrice = product.ProductVariants
                                          .Select(p => p.Price)
                                          .DefaultIfEmpty(0m) // 0m là giá trị mặc định cho decimal
                                          .Min();
                minPrices.Add(minPrice);
            }
            ViewBag.MinPrices = minPrices;
            return View(products);
        }


        public IActionResult Privacy()
        {
            ViewData["newsOnTops"] = _context.NewsOnTops.ToList();
            var product = _context.Products
                                .OrderByDescending(p => p.Id)
                                .Include(p => p.ProductVariants)
                                .FirstOrDefault();
            if (product != null)
            {
                var price = _context.ProductVariants.Where(p => p.ProductId == product.Id);
                if (price.Any())
                {
                    var priceMin = price.Min(p => p.Price);
                    ViewBag.NewProduct = product;
                    ViewBag.Price = priceMin;
                }
                else
                {
                    ViewBag.NewProduct = product;
                    ViewBag.Price = 0;
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
