using Microsoft.EntityFrameworkCore;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Interfaces;
using RetailOrdering.Infrastructure.Data;

namespace RetailOrdering.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly ApplicationDbContext _context;

    public BrandRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        return await _context.Brands
            .Where(b => b.IsActive)
            .ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<Brand> CreateAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand?> UpdateAsync(Brand brand)
    {
        var existing = await _context.Brands.FindAsync(brand.Id);
        if (existing == null)
            return null;

        existing.Name = brand.Name;
        existing.Description = brand.Description;
        existing.ImageUrl = brand.ImageUrl;
        existing.IsActive = brand.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
            return false;

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return true;
    }
}
