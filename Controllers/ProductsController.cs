using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

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
        public async Task<IActionResult> Index(string? name, decimal? Pricefrom, int? CategoryId, string? sortOrder, int? CategoryIdShow, int? page)
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var listCategories = categories.ToList();
            listCategories.Insert(0, new Category { Id = 0, Name = "All" });
            ViewBag.CategoryID = new SelectList(listCategories, "Id", "Name", CategoryId);

            Pricefrom = Pricefrom ?? 0;
            //LỌC
            if (!string.IsNullOrEmpty(name))
            {
                if (CategoryId > 0)
                {
                    products = products.Where(x => x.CategoryId == CategoryId && x.Name.Contains(name) && x.ProductVariants.Any(v => v.Price >= Pricefrom));
                }
                else
                {
                    products = products.Where(x => x.Name.Contains(name) && x.ProductVariants.Any(v => v.Price >= Pricefrom));
                }
            }
            else
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

        

                //ĐẾM SỐ LOẠI
                var categoryCounts = (from c in _categoryRepository.GetAllAsync().Result
                                      join p in products on c.Id equals p.CategoryId into productGroup
                                      from pg in productGroup.DefaultIfEmpty()
                                      group pg by new { c.Id, c.Name } into g
                                      select new
                                      {
                                          CategoryId = g.Key.Id,
                                          CategoryName = g.Key.Name,
                                          Count = g.Count(t => t != null && t.CategoryId == g.Key.Id)
                                      }).ToList();


                categoryCounts.Insert(0, new { CategoryId = 0, CategoryName = "All Categories", Count = products.Count() });
                ViewBag.CategoryCounts = categoryCounts;
                if (CategoryIdShow == null)
                {
                    CategoryIdShow = 0;
                }

                // SẮP XẾP
                switch (sortOrder)
                {
                    case "name_asc":
                        products = products.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        products = products.OrderByDescending(p => p.Name);
                        break;
                    case "price_asc":
                        products = products.OrderBy(p => p.ProductVariants.Where(variant => variant.Price >= Pricefrom).Min(v => v.Price));
                        break;
                    case "price_desc":
                        products = products.OrderByDescending(p => p.ProductVariants.Where(variant => variant.Price >= Pricefrom).Min(v => v.Price));
                        break;
                    default:
                        break;
                }


                List<decimal> minPrices = new List<decimal>();
                foreach (var product in products)
                {
                    var minPrice = product.ProductVariants
                                              .Where(variant => variant.Price >= Pricefrom)
                                              .Select(p => p.Price)
                                              .DefaultIfEmpty(0m) // 0m là giá trị mặc định cho decimal
                                              .Min();
                    minPrices.Add(minPrice);
                }
                ViewBag.MinPrices = minPrices;
                if (CategoryIdShow.HasValue && CategoryIdShow.Value > 0)
                {
                    products = products.Where(p => p.CategoryId == CategoryIdShow.Value);
                }
            ViewBag.TotalProducts = products.Count();

            //PHÂN TRANG
            var pageNumber = page ?? 1; // Nếu không có số trang được cung cấp, mặc định là trang 1
                var pageSize = 12; // Số lượng sản phẩm trên mỗi trang
                products = await products.ToPagedListAsync(pageNumber, pageSize);
                ViewBag.CategoryIdShow = CategoryIdShow;
                ViewBag.CurrentName = name;
                ViewBag.CurrentPriceFrom = Pricefrom;
                ViewBag.CurrentCategoryId = CategoryId;
                ViewBag.CurrentSortOrder = sortOrder;
                ViewBag.Page = pageNumber;
                ViewBag.PageSize = pageSize;

                return View(products);
            }   
    }
}
