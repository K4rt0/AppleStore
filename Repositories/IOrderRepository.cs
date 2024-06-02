using AppleStore.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppleStore.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<OrderStatus?> GetOrderStatusAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<Order> GetUserOrderAsync(int id);
    }
}
