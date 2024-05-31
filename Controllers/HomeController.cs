using AppleStore.Data;
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

        public HomeController(IProductRepository productRepository, IProductAttributeRepository productAttributeRepository, ICategoryRepository categoryRepository, IProductVariantRepository productVariantRepository, ApplicationDbContext context, INotyfService notyf)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productVariantRepository = productVariantRepository;
            _productAttributeRepository = productAttributeRepository;
            _context = context;
            _notyf = notyf;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["newsOnTops"] = _context.NewsOnTops.ToList();
            var products = (await _productRepository.GetAllAsync()).Where(p => p.Display == true && p.HotSeller == true).Take(8);
            if (products != null)
            {
                return View(products);
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
