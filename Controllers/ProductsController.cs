using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppleStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext dbContext)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = dbContext;
        }
        public async Task<IActionResult> Index(string? name, decimal? Priceto, decimal? Pricefrom, int? CategoryId, string? sortOrder)
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var listCategories = categories.ToList();
            listCategories.Insert(0, new Category { Id = 0, Name = "Select Category" });
            ViewBag.CategoryID = new SelectList(listCategories, "Id", "Name", CategoryId);
            if (!string.IsNullOrEmpty(name))
            {
                if (Priceto != null && Pricefrom != null)
                {
                    if (CategoryId > 0)
                    {
                        products = products.Where(x => x.CategoryId == CategoryId && x.Name.Contains(name) && x.ProductVariants.Any(v=> v.Price >= Priceto && v.Price <= Pricefrom));
                    }
                    else
                    {
                        products = products.Where(x => x.Name.Contains(name) && x.ProductVariants.Any(v => v.Price >= Priceto && v.Price <= Pricefrom));
                    }

                }
                else
                {
                    products = products.Where(x => x.Name.Contains(name));

                }
            }
            else
            {
                if (Priceto != null && Pricefrom != null)
                {
                    if (CategoryId > 0)
                    {
                        products = products.Where(x => x.CategoryId == CategoryId && x.ProductVariants.Any(v => v.Price >= Priceto && v.Price <= Pricefrom));

                    }
                    else
                    {
                        products = products.Where(x => x.ProductVariants.Any(v => v.Price >= Priceto && v.Price <= Pricefrom));
                    }
                }
                else
                {
                    products = products.Where(x => x.CategoryId == CategoryId);
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
                case "name-desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductVariants.Min(v => v.Price));
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductVariants.Min(v => v.Price));
                    break;
                default:
                    break;
            }
            return View(products);
        }   
    }
}
