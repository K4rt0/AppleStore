using AppleStore.Data;
using AppleStore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Repositories
{
    public class EFProductAttributeValueRepository : IProductAttributeValueRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductAttributeValueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductAttributeValue>> GetAllAsync()
        {
            return await _context.ProductAttributeValues
                .Include(p => p.ProductAttribute)
                .ToListAsync();
        }

        public async Task<ProductAttributeValue> GetByIdAsync(int id)
        {
            return await _context.ProductAttributeValues
                .Include(p => p.ProductAttribute)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(ProductAttributeValue productAttributeValue)
        {
            _context.ProductAttributeValues.Add(productAttributeValue);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductAttributeValue productAttributeValue)
        {
            _context.ProductAttributeValues.Update(productAttributeValue);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productAttributeValue = await _context.ProductAttributeValues.FindAsync(id);
            _context.ProductAttributeValues.Remove(productAttributeValue);
            await _context.SaveChangesAsync();
        }
    }
}
