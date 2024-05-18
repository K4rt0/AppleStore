using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppleStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductAttributeValueRepository _productAttributeValueRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        public ProductsController(IProductRepository productRepository, IProductAttributeRepository productAttributeRepository, ICategoryRepository categoryRepository, IProductVariantRepository productVariantRepository, IProductAttributeValueRepository productAttributeValueRepository, ApplicationDbContext context, INotyfService notyf)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productVariantRepository = productVariantRepository;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _context = context;
            _notyf = notyf;
        }
        /*public async Task<IActionResult> Details(int id, int? colorId)
        {
            var productVariant = await _productVariantRepository.GetByIdAsync(id);
            if (productVariant == null) return NotFound();
            var product = await _productRepository.GetByIdAsync(productVariant.ProductId);
            if (product == null) return NotFound();
            var productVariantsRelated = await _productVariantRepository.GetByProductIdAsync(product.Id);

            List<ProductVariant> _productVariantsRelated = new();
            if (productVariantsRelated != null)
            {
                foreach (var productVariantRelated in productVariantsRelated)
                {
                    var variantsAttributes = new List<VariantsAttributes>();
                    foreach (var variantAttributes in productVariantRelated.VariantsAttributes!)
                    {
                        variantsAttributes.Add(new VariantsAttributes
                        {
                            Id = variantAttributes.Id,
                            ProductAttributeValueId = variantAttributes.ProductAttributeValueId,
                        });
                    }
                    _productVariantsRelated.Add(new ProductVariant
                    {
                        Id = productVariantRelated.Id,
                        VariantsAttributes = variantsAttributes
                    });
                }
            }

            var colors = product.ProductVariants
                .SelectMany(p => p.VariantsAttributes)
                .Where(p => p.ProductAttributeValue.ProductAttribute.Name == "Màu sắc")
                .GroupBy(va => va.ProductAttributeValueId)
                .Select(p => p.First())
                .ToList();

            //var storages = new List<VariantsAttributes>();
            var storages = product.ProductVariants
                .SelectMany(p => p.VariantsAttributes)
                .Where(p => p.ProductAttributeValue.ProductAttribute.Name == "Dung lượng lưu trữ")
                .GroupBy(va => va.ProductAttributeValueId)
                .Select(p => p.First())
                .ToList();

            ViewData["productVariant"] = productVariant;
            ViewData["productVariantsRelated"] = JsonConvert.SerializeObject(_productVariantsRelated);
            ViewData["detailsColorList"] = colors;
            ViewData["detailsStorageList"] = storages;

            return View(product);
        }*/

        public async Task<IActionResult> Details(int id)
        {
            /*var productVariant = await _productVariantRepository.GetByIdAsync(id);
            if (productVariant == null) return NotFound();*/
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            var productVariantsRelated = await _productVariantRepository.GetByProductIdAsync(product.Id);

            List<ProductVariant> _productVariantsRelated = new();
            if (productVariantsRelated != null)
            {
                foreach (var productVariantRelated in productVariantsRelated)
                {
                    var variantsAttributes = new List<VariantsAttributes>();
                    foreach (var variantAttributes in productVariantRelated.VariantsAttributes!)
                    {
                        variantsAttributes.Add(new VariantsAttributes
                        {
                            Id = variantAttributes.Id,
                            ProductAttributeValueId = variantAttributes.ProductAttributeValueId,
                        });
                    }
                    _productVariantsRelated.Add(new ProductVariant
                    {
                        Id = productVariantRelated.Id,
                        VariantsAttributes = variantsAttributes
                    });
                }
            }

            var colors = product.ProductVariants
                .SelectMany(p => p.VariantsAttributes)
                .Where(p => p.ProductAttributeValue.ProductAttribute.Name == "Màu sắc")
                .GroupBy(va => va.ProductAttributeValueId)
                .Select(p => p.First())
                .ToList();

            //var storages = new List<VariantsAttributes>();
            var storages = product.ProductVariants
                .SelectMany(p => p.VariantsAttributes)
                .Where(p => p.ProductAttributeValue.ProductAttribute.Name == "Dung lượng lưu trữ")
                .GroupBy(va => va.ProductAttributeValueId)
                .Select(p => p.First())
                .ToList();
            ViewData["detailsColorList"] = colors;
            ViewData["detailsStorageList"] = storages;

            return View(product);
        }
        public async Task<IActionResult> GetStorages(int productId, int colorId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return NotFound();

            var storages = product.ProductVariants
                .SelectMany(p => p.VariantsAttributes)
                .Where(p => p.ProductAttributeValue.ProductAttribute.Name == "Dung lượng lưu trữ" && p.ProductVariant.ProductId == productId && p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == colorId))
                .GroupBy(va => va.ProductAttributeValueId)
                .Select(p => p.First())
                .ToList();

            var result = "";
            foreach (var storage in storages)
            {
                result += $@"
                    <input type='radio' class='btn-check d-block select-storage' name='storage-options' id='storage-{storage.Id}' value='{storage.ProductAttributeValueId}' autocomplete='off'>
                    <label class='btn btn-outline-success' for='storage-{storage.Id}' style='color: {storage.ProductAttributeValue.Value};'>{storage.ProductAttributeValue.Value}</label>
                ";
            }

            return Content(result, "text/html");
        }
        public async Task<IActionResult> GetPrices(int productId, int colorId, int storageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            var color = await _productAttributeValueRepository.GetByIdAsync(colorId);
            var storage = await _productAttributeRepository.GetByIdAsync(storageId);

            if (product == null)
                return NotFound();

            var variant = _context.VariantsAttributes!
                .FirstOrDefault(p =>
                    p.ProductVariant.ProductId == productId &&
                    p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == colorId) &&
                    p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == storageId)).ProductVariant;

            var result = $"<h3 class=\"price\">{variant.Price:#,##0}đ<span>$945</span></h3>";

            return Content(result, "text/html");
        }
        public async Task<IActionResult> Index(string? name, decimal? Pricefrom, int? CategoryId, string? sortOrder, int? CategoryIdShow, int? page)
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var listCategories = categories.ToList();
            listCategories.Insert(0, new Category { Id = 0, Name = "All" });
            ViewBag.CategoryID = new SelectList(listCategories, "Id", "Name", CategoryId);

            //LỌC
            if (!string.IsNullOrEmpty(name))
            {
                if (Pricefrom != null)
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
                            products = products.OrderBy(p => p.ProductVariants.Min(v => v.Price));
                            break;
                        case "price_desc":
                            products = products.OrderByDescending(p => p.ProductVariants.Min(v => v.Price));
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
