using AppleStore.Data;
using AppleStore.Models.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace AppleStore.Repositories
{
    public class EFProductVariantRepository : IProductVariantRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductVariantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync()
        {
            return await _context.ProductVariants
                .Include(p => p.VariantsAttributes)
                .Include(p => p.Product)
                .ThenInclude(p => p.ProductVariants)
                .ToListAsync();
        }

        public async Task<ProductVariant?> GetByIdAsync(int id)
        {
            return await _context.ProductVariants
                .Include(p => p.VariantsAttributes)
                .Include(p => p.Product)
                .ThenInclude(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProductVariant>> GetByProductIdAsync(int id)
        {
            return await _context.ProductVariants
                .Include(p => p.VariantsAttributes)
                .Include(p => p.Product)
                .ThenInclude(p => p.ProductVariants)
                .Where(p => p.ProductId == id).ToListAsync();
        }

        public async Task AddAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productVariant = await _context.ProductVariants.FindAsync(id);
            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();
        }
    }
}
