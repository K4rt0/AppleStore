using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminInvoicesController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AdminInvoicesController(IOrderRepository orderRepository, UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _orderRepository.GetAllOrdersAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            var productVariant = await _orderRepository.GetProductVariant(id);

            if (productVariant != null)
            {
                var color = productVariant?.ProductVariant?.VariantsAttributes?
                    .FirstOrDefault(va => va.ProductAttributeValue?.ProductAttribute?.Name == "Màu sắc")?.ProductAttributeValue?.Name;
                var storage = productVariant?.ProductVariant?.VariantsAttributes?
                    .FirstOrDefault(va => va.ProductAttributeValue?.ProductAttribute?.Name == "Dung lượng lưu trữ")?.ProductAttributeValue?.Name;

                ViewBag.Color = color;
                ViewBag.Storage = storage;
            }

            ViewBag.ProductVariant = productVariant;

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var userOrder = await _orderRepository.GetUserOrderAsync(id);
            ViewBag.OrderConfirmed = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Confirmed);
            ViewBag.OrderCancelled = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);
            if(userOrder == null)
            {
                return NotFound();
            }
            return View(userOrder);
        }

        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus newStatus)
        {
            var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
            if (result)
            {
                return RedirectToAction("Details", new { id = orderId });
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Print(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            var productVariant = await _orderRepository.GetProductVariant(id);
            
            if (productVariant != null)
            {
                var color = productVariant?.ProductVariant?.VariantsAttributes?
                    .FirstOrDefault(va => va.ProductAttributeValue?.ProductAttribute?.Name == "Màu sắc")?.ProductAttributeValue?.Name;
                var storage = productVariant?.ProductVariant?.VariantsAttributes?
                    .FirstOrDefault(va => va.ProductAttributeValue?.ProductAttribute?.Name == "Dung lượng lưu trữ")?.ProductAttributeValue?.Name;

                ViewBag.Color = color;
                ViewBag.Storage = storage;
            }
            ViewBag.SubTotal = order.TotalPrice;
            ViewBag.Tax = (order.TotalPrice * 5/100);
            ViewBag.Total = (order.TotalPrice - (order.Discount?.Price) - (order.TotalPrice * 5 / 100));
            ViewBag.ProductVariant = productVariant;
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }

    public static class StatusHelper
    {
        public static string GetStatusClass(OrderStatus? status)
        {
            switch (status)
            {
                case OrderStatus.Pending:
                    return "bg-warning";
                case OrderStatus.Confirmed:
                    return "bg-success";
                case OrderStatus.Shipped:
                    return "bg-info";
                case OrderStatus.Delivered:
                    return "bg-primary";
                case OrderStatus.Cancelled:
                    return "bg-danger";
                default:
                    return "bg-secondary";
            }
        }
        public static string GetDisplayNameStatus(OrderStatus? status)
        {
            switch (status)
            {
                case OrderStatus.Pending:
                    return "Đang chờ xử lý";
                case OrderStatus.Confirmed:
                    return "Đã xác nhận";
                case OrderStatus.Shipped:
                    return "Đã giao hàng";
                case OrderStatus.Delivered:
                    return "Đã giao đến nơi";
                case OrderStatus.Cancelled:
                    return "Đã hủy";
                default:
                    return "Không xác định";
            }
        }
    }

}
