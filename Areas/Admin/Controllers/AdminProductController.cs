using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Owner + "," + Role.Role_Admin)]
    public class AdminProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;

        public INotyfService _notyf { get; }

        public AdminProductController(IProductRepository productRepository, ICategoryRepository categoryRepository ,ApplicationDbContext context, INotyfService notyf)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
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
                discountList.Add(new SelectListItem { Value = discount.Id.ToString(), Text = $"{discount.Code} - {discount.Name}" });
            }
            ViewBag.Discounts = discountList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile avatar)
        {
            if (ModelState.IsValid)
            {
                if (avatar != null)
                {
                    product.Avatar = await SaveImage(avatar);

                }

                await _productRepository.AddAsync(product);
                _notyf.Success("Sản phẩm đã được tạo");
                return RedirectToAction(nameof(Index));
            }
            var categories = await _productRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories,"Id", "Name");
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile avatar)
        {
            ModelState.Remove("Avatar");
            if(id != product.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if(avatar == null)
                {
                    product.Avatar = existingProduct.Avatar;
                }
                else
                {
                    product.Avatar = await SaveImage(avatar);
                }

                existingProduct.Name = product.Name;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Avatar = product.Avatar;
                await _productRepository.UpdateAsync(existingProduct);
                _notyf.Success("Cập nhật sản phẩm thành công!");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public async Task<IActionResult> Details(int id)
        {
            var products = await _productRepository.GetByIdAsync(id);
            if(products == null)
            {
                return NotFound();
            }
            ViewBag.Products = products;
            return View(products);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/adminAssets/images/products", image.FileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
                return "/adminAssets/images/products/" + image.FileName;
            }
        }

    }
}
