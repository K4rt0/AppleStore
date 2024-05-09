using AppleStore.Models.Entities;

namespace AppleStore.Repositories
{
    public interface IProductAttributeRepository
    {
        Task<IEnumerable<ProductAttribute>> GetAllAsync();
        Task<ProductAttribute> GetByIdAsync(int id);
        Task<ProductAttribute> GetByNameAsync(string name);
        Task AddAsync(ProductAttribute productAttribute);
        Task UpdateAsync(ProductAttribute productAttribute);
        Task DeleteAsync(int id);
    }
}
