using RetailOrdering.Core.Entities;

namespace RetailOrdering.Core.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId);
    Task<CartItem?> GetByIdAsync(int id);
    Task<CartItem?> GetByUserAndProductAsync(int userId, int productId);
    Task<CartItem> CreateAsync(CartItem cartItem);
    Task<CartItem?> UpdateAsync(CartItem cartItem);
    Task<bool> DeleteAsync(int id);
    Task<bool> ClearCartAsync(int userId);
}
