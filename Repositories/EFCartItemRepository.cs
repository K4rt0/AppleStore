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
                .ThenInclude(p => p.VariantsAttributes)
                .ThenInclude(p => p.ProductAttributeValue)
                .ThenInclude(p => p.ProductAttribute)
                .Include(p => p.ProductVariant)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.Category)
                .Include(p => p.ProductVariant)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.Discount)
                .ToListAsync();
        }
        public async Task<int> GetProductVariantByAttributeIdAsync(int product, int colorId, int storageId)
        {
            if (colorId == 0 && storageId == 0)
                return await _context.ProductVariants
                    .Where(p => p.ProductId == product)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
            else
                return await _context.ProductVariants
                    .Where(p => p.ProductId == product && p.VariantsAttributes.Any(va => va.ProductAttributeValue.Id == colorId) && p.VariantsAttributes.Any(va => va.ProductAttributeValue.Id == storageId))
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<CartItem>> GetAllByUserIdAsync(string userId)
        {
            return await _context.CartItems
                .Include(p => p.ProductVariant)
                .ThenInclude(p => p.VariantsAttributes)
                .ThenInclude(p => p.ProductAttributeValue)
                .ThenInclude(p => p.ProductAttribute)
                .Include(p => p.ProductVariant)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.Category)
                .Include(p => p.ProductVariant)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.Discount)
                .Where(p => userId == p.ApplicationUserId)
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
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId && p.ProductVariantId == productVariantId);
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
