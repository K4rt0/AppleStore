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
                    products = products.Where(x => x.CategoryId == CategoryId && x.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0 && x.ProductVariants.Any(v => v.Price >= Pricefrom));
                }
                else
                {
                    products = products.Where(x => x.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0 && x.ProductVariants.Any(v => v.Price >= Pricefrom));
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
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();


            var colors = product.ProductVariants
                .SelectMany(p => p.VariantsAttributes)
                .Where(p => p.ProductAttributeValue.ProductAttribute.Name == "Màu sắc")
                .GroupBy(va => va.ProductAttributeValueId)
                .Select(p => p.First())
                .ToList();

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
        /* [HttpPost]
        public async Task<JsonResult> GetColors(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if(product == null) return Json(null);
            var colors = product.ProductVariants
                .SelectMany(pv => pv.VariantsAttributes)
                .Where(va => va.ProductAttributeValue.ProductAttribute.Name == "Màu sắc")
                .Select(va => new {
                    id = va.ProductAttributeValueId,
                    value = va.ProductAttributeValue.Value
                })
                .Distinct()
                .ToList();
            return Json(new { colors });
        }
        [HttpPost]
        public async Task<JsonResult> GetStorages(int productId, int colorId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return Json(null);

            var storages = product.ProductVariants
                .SelectMany(p => p.VariantsAttributes)
                .Where(p => p.ProductAttributeValue.ProductAttribute.Name == "Dung lượng lưu trữ" && p.ProductVariant.ProductId == productId && p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == colorId))
                .Select(p => new {
                    id = p.Id,
                    productAttributeValueId = p.ProductAttributeValueId,
                    value = p.ProductAttributeValue.Value,
                    price = p.ProductVariant.Price
                })
                .Distinct()
                .ToList();

            return Json(new { storages });
        } */
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
            if (storages != null && storages.Count > 0)
            {
                result += $"<label class=\"title-label\">Dung lượng</label>";
                foreach (var storage in storages)
                {
                    result += $@"
                        <input type='radio' class='btn-check d-block select-storage' name='storage-options' id='storage-{storage.Id}' value='{storage.ProductAttributeValueId}' autocomplete='off'>
                        <label class='btn btn-outline-success' for='storage-{storage.Id}' style='color: {storage.ProductAttributeValue.Value};'>{storage.ProductAttributeValue.Value}</label>
                    ";
                }
            }
            return Content(result, "text/html");
        }
        public async Task<IActionResult> GetPrices(int productId, int colorId, int storageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                return NotFound();

            var variant = _context.VariantsAttributes
                        .FirstOrDefault(p =>
                            p.ProductVariant.ProductId == productId &&
                            p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == colorId) &&
                            p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == storageId)).ProductVariant;

            decimal discountApply = product.Discount != null ? (product.Discount.Price > 0 ? product.Discount.Price : (product.Discount.Percent > 0 ? variant.Price * (product.Discount.Percent / (Decimal)100) : 0)) : 0;
            string discount = discountApply != 0 ? $"<span>{variant.Price:#,##0}đ</span>" : "";
            var result = $"<h3 class='price'>{variant.Price - discountApply:#,##0}đ{discount}</h3>";

            return Content(result, "text/html");
        }
        public async Task<IActionResult> GetQuantityStatus(int productId, int colorId, int storageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            var color = await _productAttributeValueRepository.GetByIdAsync(colorId);
            var storage = await _productAttributeRepository.GetByIdAsync(storageId);

            if (product == null)
                return NotFound();

            var variant = _context.VariantsAttributes
                .FirstOrDefault(p =>
                    p.ProductVariant.ProductId == productId &&
                    p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == colorId) &&
                    p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == storageId)).ProductVariant;

            string result = "";

            if (variant.Quantity > 1)
            {
                result = $@"
                    <div class='category text-success'>
                        <i class='lni lni-thumbs-up'></i> Còn hàng
                    </div>";
            }
            else
            {
                result = $@"
                    <div class='category text-danger'>
                        <i class='lni lni-thumbs-down'></i> Hết hàng
                    </div>";
            }

            return Content(result, "text/html");
        }

        public async Task<IActionResult> GetProductDescription(int productId, int colorId, int storageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            var color = await _productAttributeValueRepository.GetByIdAsync(colorId);
            var storage = await _productAttributeRepository.GetByIdAsync(storageId);

            if (product == null)
                return Content("", "text/html");


            ProductVariant variant = new ProductVariant();
            List<VariantsAttributes>? attributes = new List<VariantsAttributes>();
            if (product.ProductVariants.Count() > 0 && product.ProductVariants.FirstOrDefault().VariantsAttributes.Count() != 0)
            {
                variant = _context.VariantsAttributes
                    .FirstOrDefault(p =>
                        p.ProductVariant.ProductId == productId &&
                        p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == colorId) &&
                        p.ProductVariant.VariantsAttributes.Any(va => va.ProductAttributeValueId == storageId)).ProductVariant;

                attributes = variant.VariantsAttributes.Where(p => p.ProductVariantId == variant.Id).ToList();
            }

            string result = "";

            if (attributes.Count != 0)
            {
                result = $@"
                    <div class='row'>
                        <div class='col-lg-8 col-12'>
                            <div class='card p-3 shadow' style='border-radius: 10px;'>
                                {product.Description}
                            </div>
                        </div>
                        <div class='col-lg-4 col-12'>
                            <div class='info-body'>
                                <div class='card p-3 shadow' style='border-radius: 10px;'>
                                    <h6>Thông số kĩ thuật</h6>
                                    <table class='mt-3 table table-striped'>
                                    <tbody>";
                foreach (var item in attributes)
                {
                    result += $@"
                        <tr>
                        <th scope='row'>{item.ProductAttributeValue.ProductAttribute.Name}</th>
                        <td>{item.ProductAttributeValue.Name}</td>
                    </tr>";
                }
                result += "</tbody></table></div></div></div></div>";
            }
            else
            {
                result = $@"
                    <div class='card p-3 shadow' style='border-radius: 10px;'>{product.Description}</div>";
            }

            return Content(result, "text/html");
        }
    }
}