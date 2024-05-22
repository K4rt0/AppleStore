using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> GetAttributes(int productId, int colorId, int storageId)
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

            var attributes = variant.VariantsAttributes.Where(p => p.ProductVariantId == variant.Id).ToList();

            var result = "";
            foreach (var item in attributes)
            {
                result += $@"
                    <tr>
                    <th scope='row'>{item.ProductAttributeValue.ProductAttribute.Name}</th>
                    <td>{item.ProductAttributeValue.Name}</td>
                </tr>";
            }
            result += "</ul>";
            return Content(result, "text/html");
        }

    }
}
