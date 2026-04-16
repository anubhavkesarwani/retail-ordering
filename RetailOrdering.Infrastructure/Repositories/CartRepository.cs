using Microsoft.EntityFrameworkCore;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Interfaces;
using RetailOrdering.Infrastructure.Data;

namespace RetailOrdering.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId)
    {
        return await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Product)
            .ThenInclude(p => p!.Category)
            .ToListAsync();
    }

    public async Task<CartItem?> GetByIdAsync(int id)
    {
        return await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == id);
    }

    public async Task<CartItem?> GetByUserAndProductAsync(int userId, int productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
    }

    public async Task<CartItem> CreateAsync(CartItem cartItem)
    {
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        // Reload with product
        return await GetByIdAsync(cartItem.Id) ?? cartItem;
    }

    public async Task<CartItem?> UpdateAsync(CartItem cartItem)
    {
        var existing = await _context.CartItems.FindAsync(cartItem.Id);
        if (existing == null)
            return null;

        existing.Quantity = cartItem.Quantity;
        existing.UpdatedAt = cartItem.UpdatedAt;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(existing.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem == null)
            return false;

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
        return true;
    }
}
