using AppleStore.Data;
using AppleStore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Repositories
{
    public class EFCartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            return await _context.CartItems
                .Include(p => p.ProductVariant)
                .Include(p => p.ProductVariant.Product)
                .Include(p => p.ProductVariant.Product.Category)
                .Include(p => p.ProductVariant.Product.Discount)
                .ToListAsync();
        }

        public async Task<IEnumerable<CartItem>> GetAllByUserIdAsync(string userId)
        {
            return await _context.CartItems
                .Include(p => p.ProductVariant)
                .Include(p => p.ProductVariant.Product)
                .Include(p => p.ProductVariant.Product.Category)
                .Include(p => p.ProductVariant.Product.Discount)
                .Where(p => userId == p.UserId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(int id)
        {
            return await _context.CartItems
                .Include(p => p.ProductVariant)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<CartItem?> GetByIdAndUserIdAsync(int productVariantId, string userId)
        {
            return await _context.CartItems
                .Include(p => p.ProductVariant)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ProductVariantId == productVariantId);
        }

        public async Task AddAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cartItems = await _context.CartItems.FindAsync(id);
            _context.CartItems.Remove(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
