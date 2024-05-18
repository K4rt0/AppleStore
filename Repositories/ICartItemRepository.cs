using AppleStore.Models.Entities;

namespace AppleStore.Repositories
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetAllAsync();
        Task<IEnumerable<CartItem>> GetAllByUserIdAsync(string userId);
        Task<CartItem?> GetByIdAsync(int id);
        Task<CartItem?> GetByIdAndUserIdAsync(int productVariantId, string userId);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task DeleteAsync(int id);
    }
}
