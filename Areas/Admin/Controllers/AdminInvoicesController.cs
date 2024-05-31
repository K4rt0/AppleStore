using AppleStore.Models.Entities;
using AppleStore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminInvoicesController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        public AdminInvoicesController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _orderRepository.GetAllOrdersAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
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
