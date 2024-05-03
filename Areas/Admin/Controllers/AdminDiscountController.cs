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
    [Authorize(Roles = Role.Role_Admin + "," + Role.Role_Owner)]
    public class AdminDiscountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDiscountRepository _discountRepository;
        private readonly INotyfService _notyf;
        public AdminDiscountController(ApplicationDbContext context, INotyfService notyf, IDiscountRepository discountRepository)
        {
            _context = context;
            _notyf = notyf;
            _discountRepository = discountRepository;
        }
        public IActionResult Index()
        {
            var discount = _context.Discounts.ToList();
            return View(discount);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Discount discount)
        {
            if (discount.Code == null)
                _notyf.Warning("Mã giảm giá không được bỏ trống !");
            else if (discount.Expire < DateTime.Now)
                _notyf.Warning("Thời gian hết hạn của mã giảm giá không được thấp hơn hiện tại !");
            else if (await _discountRepository.FindAsync(discount.Code))
                _notyf.Warning("Mã giảm giá này đã tồn tại trong hệ thống !");
            else if (discount.Name == null)
                _notyf.Warning("Bạn phải đặt tên cho mã giảm giá !");
            else if (discount.Quantity == 0)
                _notyf.Warning("Mã giảm giá phải có số lượng !");
            else if ((discount.Percent == 0 && discount.Price == 0) || discount.Percent < 0 || discount.Price < 0)
                _notyf.Warning("Giảm giá không hợp lệ !");
            else if (discount.Percent != 0 && discount.Price != 0)
                _notyf.Warning("Bạn chỉ có thể chọn một trong hai !");
            else
            {
                if (!ModelState.IsValid || discount == null)
                    return NotFound();

                await _context.Discounts.AddAsync(discount);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }    
            return View(discount);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            if (discount == null)
                return NotFound();
            return View(discount);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Discount discount)
        {
            if (id != discount.Id)
                return NotFound();
            var existingDiscount = await _discountRepository.GetByIdAsync(id);

            existingDiscount.Name = discount.Name;
            existingDiscount.Code = discount.Code;
            existingDiscount.Expire = discount.Expire;
            existingDiscount.Price = discount.Price;
            existingDiscount.Percent = discount.Percent;
            existingDiscount.Quantity = discount.Quantity;
            existingDiscount.Active = discount.Active;

            await _discountRepository.UpdateAsync(existingDiscount);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _discountRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
