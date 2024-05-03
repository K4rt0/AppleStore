namespace AppleStore.Repositories
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<Discount> GetByIdAsync(int id);
        Task<bool> FindAsync(string code);
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(int id);
    }
}
