using AppleStore.Data;
using AppleStore.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.ApplicationUser).ToListAsync();
        }
        public async Task<List<Order>> GetAllOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders.Where(o => o.ApplicationUserId == userId).ToListAsync();
        }
        public async Task<OrderDetail> GetProductVariant(int id)
        {
            return await _context.OrderDetails
                .Include(od => od.ProductVariant)
                .ThenInclude(pv => pv.VariantsAttributes)
                .ThenInclude(va => va.ProductAttributeValue)
                .ThenInclude(pav => pav.ProductAttribute)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                //.Include(o => o.ApplicationUser)
                //.Include(o => o.OrderDetails)
                //.ThenInclude(p => p.Product)
                //.ThenInclude(p => p.Category)
                //.Include(o => o.OrderDetails)
                //.ThenInclude(od => od.Product)
                //.ThenInclude(p => p.ProductVariants)
                //.Include(o => o.OrderDetails)
                //.ThenInclude(od => od.Product)
                //.ThenInclude(p => p.Discount)
                //.ThenInclude(od => od.ProductVariant)
                //.ThenInclude(od => od.Product)
                //.Include(o => o.DeliveryAddress)
                //.FirstOrDefaultAsync(o => o.Id == id);
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(od => od.Product)
                .ThenInclude(od => od.Category)
                .ThenInclude(od => od.Discount)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(od => od.Product)
                .Include(o => o.DeliveryAddress)
                .Where(o => o.Status == status)
                .ToListAsync();
        }
        public async Task<OrderStatus?> GetOrderStatusAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            return order?.Status;
        }
        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }
            order.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Order> GetUserOrderAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.ApplicationUser)
                .FirstAsync(o => o.Id == id);
        }
    }
}
