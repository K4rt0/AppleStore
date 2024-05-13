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
    }
}
