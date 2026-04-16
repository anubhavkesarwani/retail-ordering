using RetailOrdering.Core.Entities;

namespace RetailOrdering.Core.Interfaces;

public interface IPackagingRepository
{
    Task<IEnumerable<Packaging>> GetAllAsync();
    Task<Packaging?> GetByIdAsync(int id);
    Task<Packaging> CreateAsync(Packaging packaging);
    Task<Packaging?> UpdateAsync(Packaging packaging);
    Task<bool> DeleteAsync(int id);
}
