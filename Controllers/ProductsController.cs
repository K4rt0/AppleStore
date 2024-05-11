using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppleStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        public INotyfService _notyf { get; }

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext dbContext, INotyfService notyfService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = dbContext;
            _notyf = notyfService;
        }
        public async Task<IActionResult> Index(string? name, decimal? Pricefrom, int? CategoryId, string? sortOrder)
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var listCategories = categories.ToList();
            listCategories.Insert(0, new Category { Id = 0, Name = "All" });
            ViewBag.CategoryID = new SelectList(listCategories, "Id", "Name", CategoryId);
            if (!string.IsNullOrEmpty(name))
            {
                if ( Pricefrom != null)
                {
                    if (CategoryId > 0)
                    {
                        products = products.Where(x => x.CategoryId == CategoryId && x.Name.Contains(name) && x.ProductVariants.Any(v=> v.Price >= Pricefrom));
                    }
                    else
                    {
                        products = products.Where(x => x.Name.Contains(name) && x.ProductVariants.Any(v => v.Price >= Pricefrom));
                    }

                }
                else
                {
                    products = products.Where(x => x.Name.Contains(name));

                }
            }
            else
            {
                if (Pricefrom != null)
                {
                    if (CategoryId > 0)
                    {
                        products = products.Where(x => x.CategoryId == CategoryId && x.ProductVariants.Any(v => v.Price >= Pricefrom));

                    }
                    else
                    {
                        products = products.Where(x => x.ProductVariants.Any(v => v.Price >= Pricefrom));
                    }
                }
                else
                {
                    if (CategoryId > 0)
                    {
                        products = products.Where(x => x.CategoryId == CategoryId);
                    }
                }

            }
            var categoryCounts = products
                    .GroupBy(p => p.Category)
                    .Select(g => new { CategoryId = g.Key.Id, CategoryName = g.Key.Name, Count = g.Count() })
                    .ToList();
            ViewBag.CategoryCounts = categoryCounts;
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductVariants.Min(v => v.Price));
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductVariants.Min(v => v.Price));
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
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
    }
}
