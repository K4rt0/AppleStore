using AppleStore.Data;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Repositories
{
    public class EFDiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllAsync()
        {
            return await _context.Discounts
                        .Include(d => d.Categories)
                        .Include(d => d.Products)
                        .Include(d => d.Orders)
                        .ToListAsync();

        }

        public async Task<Discount> GetByIdAsync(int id)
        {
            return await _context.Discounts
                        .Include(d => d.Categories)
                        .Include(d => d.Products)
                        .Include(d => d.Orders)
                        .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var discount = await _context.Discounts
                        .Include(d => d.Categories)
                        .Include(d => d.Products)
                        .Include(d => d.Orders)
                        .FirstOrDefaultAsync(p => p.Id == id);
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> FindAsync(string code)
        {
            return await _context.Discounts.Include(d => d.Categories)
                        .Include(d => d.Products)
                        .Include(d => d.Orders)
                        .AnyAsync(p => p.Code == code);
        }
    }
}
