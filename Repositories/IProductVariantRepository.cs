using AppleStore.Models.Entities;

namespace AppleStore.Repositories
{
    public interface IProductVariantRepository
    {
        Task<IEnumerable<ProductVariant>> GetAllAsync();
        Task<ProductVariant?> GetByIdAsync(int id);
        Task<List<ProductVariant>> GetByProductIdAsync(int id);
        Task AddAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task DeleteAsync(int id);
    }
}
