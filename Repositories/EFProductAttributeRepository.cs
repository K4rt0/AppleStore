using AppleStore.Data;
using AppleStore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Repositories
{
    public class EFProductAttributeRepository : IProductAttributeRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductAttributeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductAttribute>> GetAllAsync()
        {
            return await _context.ProductAttributes.Include(p => p.ProductAttributeValues).ToListAsync();
        }

        public async Task<ProductAttribute> GetByIdAsync(int id)
        {
            return await _context.ProductAttributes.Include(p => p.ProductAttributeValues).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<ProductAttribute> GetByNameAsync(string name)
        {
            return await _context.ProductAttributes.Include(p => p.ProductAttributeValues).FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task AddAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Add(productAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Update(productAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productAttribute = await _context.ProductAttributes.FindAsync(id);
            _context.ProductAttributes.Remove(productAttribute);
            await _context.SaveChangesAsync();
        }
    }
}
