using RetailOrdering.Core.Entities;

namespace RetailOrdering.Core.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByBrandIdAsync(int brandId);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}
