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
                .ToListAsync();
        }

        public async Task<CartItem> GetByIdAsync(int id)
        {
            return await _context.CartItems
                .Include(p => p.ProductVariant)
                .FirstOrDefaultAsync(p => p.Id == id);
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
