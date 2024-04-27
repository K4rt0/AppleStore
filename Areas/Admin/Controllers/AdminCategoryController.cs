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
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;

        public INotyfService _notyf { get; }
        public AdminCategoryController(ICategoryRepository categoryRepository, ApplicationDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var category = await _categoryRepository.GetAllAsync();
            if (category != null)
                return View(category);
            return View();
        }

        public IActionResult Create()
        {
            var discount = _context.Discounts.ToList();
            ViewBag.Discounts = new SelectList(discount, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid || category == null)
                return NotFound();
            if (CategoryExists(category.Name))
            {
                _notyf.Warning("Danh mục này đã tồn tại trong hệ thống !");
                return View(category);
            }
            await _categoryRepository.AddAsync(category);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
                return NotFound();
            var existingCategory = await _categoryRepository.GetByIdAsync(id);

            existingCategory.Name = category.Name;
            existingCategory.Display = category.Display;

            if(existingCategory.DiscountId != category.DiscountId)
            {
                existingCategory.DiscountId = category.DiscountId;
                existingCategory.Discount = category.Discount;
            }
            await _categoryRepository.UpdateAsync(existingCategory);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public bool CategoryExists(string name)
        {
            var item = _context.Categories.FirstOrDefault(p => p.Name == name);
            if (item != null)
                return true;
            return false;
        }
    }
}
