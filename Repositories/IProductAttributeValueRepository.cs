using AppleStore.Models.Entities;

namespace AppleStore.Repositories
{
    public interface IProductAttributeValueRepository
    {
        Task<IEnumerable<ProductAttributeValue>> GetAllAsync();
        Task<ProductAttributeValue> GetByIdAsync(int id);
        Task AddAsync(ProductAttributeValue productAttributeValue);
        Task UpdateAsync(ProductAttributeValue productAttributeValue);
        Task DeleteAsync(int id);
    }
}
