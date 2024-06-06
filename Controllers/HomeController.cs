using AppleStore.Data;
using AppleStore.Models;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace AppleStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        public HomeController(IProductRepository productRepository, 
            IProductAttributeRepository productAttributeRepository, 
            ICategoryRepository categoryRepository, 
            IProductVariantRepository productVariantRepository, 
            ApplicationDbContext context, INotyfService notyf,
            IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productVariantRepository = productVariantRepository;
            _productAttributeRepository = productAttributeRepository;
            _orderRepository = orderRepository;
            _context = context;
            _notyf = notyf;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["newsOnTops"] = _context.NewsOnTops.ToList();
            var products = (await _productRepository.GetAllAsync()).Where(p => p.Display == true && p.HotSeller == true).Take(8);
            if (products != null)
            {
                return View(products);
            }
            return View();
        }

        public async Task<IActionResult> MyOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderList = await _orderRepository.GetAllOrdersByUserIdAsync(userId);
            ViewBag.UserName = _context.ApplicationUsers.First(a => a.Id == userId).FullName;
            ViewBag.OrderCount = await _context.Orders.CountAsync();
            ViewBag.OrderPending = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            ViewBag.OrderCancelled = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);
            if(orderList == null)
            {
                return NotFound();
            }
            return View(orderList);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var orderDetails = await _orderRepository.GetOrderByIdAsync(id);
            if (orderDetails == null)
            {
                return NotFound();
            }
            return View(orderDetails);
        }

        public async Task<IActionResult> UpdateStatus(int orderId)
        {
            var orderCancelled = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderCancelled != null && orderCancelled.Status != OrderStatus.Cancelled)
            {
                orderCancelled.Status = OrderStatus.Cancelled;
                _context.SaveChanges();
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            else
            {
                return NotFound();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
