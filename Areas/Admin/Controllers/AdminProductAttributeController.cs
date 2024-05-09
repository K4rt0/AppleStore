using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Owner + "," + Role.Role_Admin)]
    public class AdminProductAttributeController : Controller
    {
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductAttributeValueRepository _productAttributeValueRepository;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        public AdminProductAttributeController(IProductAttributeRepository productAttributeRepository, IProductAttributeValueRepository productAttributeValueRepository, INotyfService notyf, ApplicationDbContext context)
        {
            _productAttributeRepository = productAttributeRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _notyf = notyf;
            _context = context;
        }
        public bool AttributeExists(string name)
        {
            return _context.ProductAttributes.FirstOrDefault(p => p.Name == name) != null;
        }
        public bool AttributeValueExists(string name, int id)
        {
            return _context.ProductAttributeValues.FirstOrDefault(p => p.Name == name && p.ProductAttributeId == id) != null;
        }


        public async Task<IActionResult> Index()
        {
            var productAttribute = await _productAttributeRepository.GetAllAsync();
            return View(productAttribute);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductAttribute productAttribute)
        {
            if (!ModelState.IsValid || productAttribute == null)
                return NotFound();
            if (AttributeExists(productAttribute.Name))
            {
                _notyf.Warning("Thuộc tính này đã tồn tại trong hệ thống !");
                return View(productAttribute);
            }
            await _productAttributeRepository.AddAsync(productAttribute);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var productAttribute = await _productAttributeRepository.GetByIdAsync(id);
            if (productAttribute == null)
                return NotFound();
            return View(productAttribute);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductAttribute productAttribute)
        {
            if (id != productAttribute.Id)
                return NotFound();
            var existingAttribute = await _productAttributeRepository.GetByIdAsync(id);

            existingAttribute.Name = productAttribute.Name;
            existingAttribute.Description = productAttribute.Description;
            await _productAttributeRepository.UpdateAsync(existingAttribute);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var productAttributeValues = _context.ProductAttributeValues.Where(p => p.ProductAttributeId == id).ToList();
            ViewBag.AttributeId = id;
            ViewBag.AttributeName = _productAttributeRepository.GetByIdAsync(id).Result.Name;
            return View(productAttributeValues);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productAttributeRepository.DeleteAsync(id);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        /* Product Attribute Value Area */
        public async Task<IActionResult> CreateValue(int id)
        {
            var item = new ProductAttributeValue()
            {
                ProductAttribute = await _productAttributeRepository.GetByIdAsync(id)
            };

            ViewBag.AttributeId = id;
            ViewBag.Color = item.ProductAttribute.Name == "Màu sắc";

            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> CreateValue(ProductAttributeValue productAttributeValue, int id)
        {
            if (!ModelState.IsValid || productAttributeValue == null)
                return NotFound();
            if (AttributeValueExists(productAttributeValue.Name, id))
            {
                ViewBag.AttributeId = id;
                ViewBag.Color = _productAttributeRepository.GetByIdAsync(id).Result.Name == "Màu sắc";
                _notyf.Warning("Thuộc tính này đã tồn tại trong hệ thống !");
                return View(productAttributeValue);
            }
            productAttributeValue.Id = 0;
            productAttributeValue.ProductAttributeId = id;
            productAttributeValue.ProductAttribute = await _productAttributeRepository.GetByIdAsync(id);
            await _productAttributeValueRepository.AddAsync(productAttributeValue);
            return RedirectToAction("Details", new { id = id });
        }
        public async Task<IActionResult> EditValue(int id)
        {
            var productAttributeValue = await _productAttributeValueRepository.GetByIdAsync(id);
            if (productAttributeValue == null)
                return NotFound();
            ViewBag.AttributeId = id;
            ViewBag.Color = productAttributeValue.ProductAttribute.Name == "Màu sắc";
            return View(productAttributeValue);
        }
        [HttpPost]
        public async Task<IActionResult> EditValue(int id, ProductAttributeValue productAttributeValue)
        {
            if (id != productAttributeValue.Id)
                return NotFound();
            var existingAttributeValue = await _productAttributeValueRepository.GetByIdAsync(id);

            existingAttributeValue.Name = productAttributeValue.Name;
            existingAttributeValue.Value = productAttributeValue.Value;
            await _productAttributeValueRepository.UpdateAsync(existingAttributeValue);
            return RedirectToAction("Details", new { id = existingAttributeValue.ProductAttributeId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteValueConfirmed(int id)
        {
            await _productAttributeValueRepository.DeleteAsync(id);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
