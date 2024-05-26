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
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        public HomeController(IProductRepository productRepository, IProductAttributeRepository productAttributeRepository, ICategoryRepository categoryRepository, IProductVariantRepository productVariantRepository, ApplicationDbContext context, INotyfService notyf)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productVariantRepository = productVariantRepository;
            _productAttributeRepository = productAttributeRepository;
            _context = context;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
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
            var products = (await _productRepository.GetAllAsync()).Where(p => p.Display == true && p.HotSeller == true).Take(8);
            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
