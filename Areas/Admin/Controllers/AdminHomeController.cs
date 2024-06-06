using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Admin + "," + Role.Role_Owner)]

    public class AdminHomeController : Controller { 

        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminHomeController (IProductRepository productRepository, 
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var productCount = await _context.Products.CountAsync();
            var userCount = await _userManager.GetUsersInRoleAsync(Role.Role_Customer);
            var orderCount = await _context.Orders.CountAsync();
            var orderTotal = await _context.Orders.SumAsync(o => o.TotalPrice);
            ViewBag.ProductCount = productCount;
            ViewBag.UserCount = userCount.Count;
            ViewBag.OrderCount = orderCount;
            ViewBag.OrderTotal = orderTotal;

            var orderStats = await _context.Orders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    OrderCount = g.Count(),
                    CategoryStats = g.SelectMany(o => o.OrderDetails)
                                     .GroupBy(od => od.ProductVariant.Product.Category.Name)
                                     .Select(cg => new
                                     {
                                         CategoryName = cg.Key,
                                         QuantitySold = cg.Sum(od => od.Quantity)
                                     })
                                     .OrderByDescending(cs => cs.QuantitySold)
                                     .ToList()
                })
                .ToListAsync();

            return View(orderStats);
        }
    }
}