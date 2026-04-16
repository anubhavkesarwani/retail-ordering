using Microsoft.EntityFrameworkCore;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Interfaces;
using RetailOrdering.Infrastructure.Data;

namespace RetailOrdering.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.Brand)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetByBrandIdAsync(int brandId)
    {
        return await _context.Categories
            .Include(c => c.Brand)
            .Where(c => c.BrandId == brandId)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Brand)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        var existing = await _context.Categories.FindAsync(category.Id);
        if (existing == null)
            return null;

        existing.Name = category.Name;
        existing.Description = category.Description;
        existing.BrandId = category.BrandId;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}
