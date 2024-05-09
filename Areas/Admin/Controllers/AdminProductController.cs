using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NuGet.Packaging;
using System.Collections.ObjectModel;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Owner + "," + Role.Role_Admin)]
    public class AdminProductController : Controller
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;

        public INotyfService _notyf { get; }

        public AdminProductController(IProductRepository productRepository, IProductAttributeRepository productAttributeRepository, ICategoryRepository categoryRepository, IProductVariantRepository productVariantRepository, ApplicationDbContext context, INotyfService notyf)
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
            var product = await _productRepository.GetAllAsync();
            ViewBag.Products = product;
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var category = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(category, "Id", "Name");
            var discounts = _context.Discounts.ToList();
            var discountList = new List<SelectListItem>();
            foreach (var discount in discounts)
            {
                if (discount.Active == true)
                    discountList.Add(new SelectListItem { Value = discount.Id.ToString(), Text = $"{discount.Code} - {discount.Name}" });
            }
            ViewBag.Discounts = discountList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile avatar, List<IFormFile> productImages)
        {
            if (ModelState.IsValid)
            {
                if (productImages != null)
                {
                    int i = 0;
                    product.ProductImages = new List<ProductImage>();
                    foreach (var img in productImages)
                    {
                        product.ProductImages.Add(new ProductImage()
                        {
                            ProductId = product.Id,
                            ImageUrl = await CommonFunc.UploadFile(img, "product-details", CommonFunc.SEOUrl(product.Name) + $"-{++i}" + Path.GetExtension(avatar.FileName))
                        });
                    }
                }
                product.Avatar = await CommonFunc.UploadFile(avatar, "products", CommonFunc.SEOUrl(product.Name) + Path.GetExtension(avatar.FileName));

                await _productRepository.AddAsync(product);
                _notyf.Success("Sản phẩm đã được tạo !");
                return RedirectToAction(nameof(Index));
            }
            var categories = await _productRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var discounts = _context.Discounts.ToList();
            var discountList = new List<SelectListItem>();
            foreach (var discount in discounts)
            {
                if (discount.Active == true)
                    discountList.Add(new SelectListItem { Value = discount.Id.ToString(), Text = $"{discount.Code} - {discount.Name}" });
            }
            ViewBag.Discounts = discountList;
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile avatar, List<IFormFile> productImages)
        {
            ModelState.Remove("Avatar");
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (productImages != null)
                {
                    product.ProductImages = new List<ProductImage>();
                    int i = existingProduct.ProductImages.Count();
                    foreach (var img in productImages)
                    {
                        existingProduct.ProductImages.Add(new ProductImage()
                        {
                            ProductId = product.Id,
                            ImageUrl = await CommonFunc.UploadFile(img, "product-details", CommonFunc.SEOUrl(product.Name) + $"-{++i}" + Path.GetExtension(img.FileName))
                        });
                    }
                }
                if (avatar != null)
                {
                    product.Avatar = await CommonFunc.UploadFile(avatar, "products", CommonFunc.SEOUrl(product.Name) + Path.GetExtension(avatar.FileName));
                    existingProduct.Avatar = product.Avatar;
                }
                existingProduct.Name = product.Name;
                existingProduct.CategoryId = product.CategoryId;
                await _productRepository.UpdateAsync(existingProduct);
                _notyf.Success("Cập nhật sản phẩm thành công !");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            var productAttributes = await _productAttributeRepository.GetAllAsync();
            var attributeNames = _context.ProductVariants
                                .Where(pv => pv.ProductId == id)
                                .SelectMany(pv => pv.VariantsAttributes)
                                .Select(va => va.ProductAttributeValue.ProductAttribute)
                                .Distinct()
                                .ToList();
            ViewBag.VariantId = id;
            ViewBag.VariantName = product.Name;
            ViewData["productVariants"] = product.ProductVariants;
            ViewData["productAttributes"] = attributeNames;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            var idTmp = image.ProductId;
            if (image == null)
                return NotFound();
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "AdminProduct", new { id = idTmp });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /* Product Variant Area */
        public async Task<IActionResult> CreateVariant(int id)
        {
            var productAttributes = await _productAttributeRepository.GetAllAsync();
            ViewBag.VariantId = id;
            ViewData["productAttributes"] = productAttributes;
            /*ViewBag.Colors = new SelectList(_context.ProductAttributeValues.Where(pav => pav.ProductAttributeId == _context.ProductAttributes.FirstOrDefault(pa => pa.NameSuggest == "Color").Id), "Id", "Value");
            ViewBag.DisplaySizes = new SelectList(_context.ProductAttributeValues.Where(pav => pav.ProductAttributeId == _context.ProductAttributes.FirstOrDefault(pa => pa.NameSuggest == "DisplaySize").Id), "Id", "Name");*/
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateVariant(ProductVariant productVariant, List<int> VariantsAttributes)
        {
            if (!ModelState.IsValid || productVariant == null)
                return NotFound();

            int id = productVariant.Id;
            var product = await _productRepository.GetByIdAsync(id);
            ICollection<VariantsAttributes> variantAttributes = new List<VariantsAttributes>();

            foreach (var item in VariantsAttributes)
            {
                if(item != 0)
                {
                    variantAttributes.Add(new VariantsAttributes()
                    {
                        ProductVariantId = 0,
                        ProductAttributeValueId = item
                    });
                }
            }
            productVariant.Id = 0;
            productVariant.ProductId = id;
            productVariant.VariantsAttributes = variantAttributes;
            await _productVariantRepository.AddAsync(productVariant);
            ViewBag.VariantName = product.Name;
            return RedirectToAction("Details", new { id = id });
        }
    }
}
