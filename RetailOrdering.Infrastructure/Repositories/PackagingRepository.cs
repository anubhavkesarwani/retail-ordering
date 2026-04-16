using Microsoft.EntityFrameworkCore;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Interfaces;
using RetailOrdering.Infrastructure.Data;

namespace RetailOrdering.Infrastructure.Repositories;

public class PackagingRepository : IPackagingRepository
{
    private readonly ApplicationDbContext _context;

    public PackagingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Packaging>> GetAllAsync()
    {
        return await _context.Packagings
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<Packaging?> GetByIdAsync(int id)
    {
        return await _context.Packagings.FindAsync(id);
    }

    public async Task<Packaging> CreateAsync(Packaging packaging)
    {
        _context.Packagings.Add(packaging);
        await _context.SaveChangesAsync();
        return packaging;
    }

    public async Task<Packaging?> UpdateAsync(Packaging packaging)
    {
        var existing = await _context.Packagings.FindAsync(packaging.Id);
        if (existing == null)
            return null;

        existing.Name = packaging.Name;
        existing.Description = packaging.Description;
        existing.IsActive = packaging.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var packaging = await _context.Packagings.FindAsync(id);
        if (packaging == null)
            return false;

        _context.Packagings.Remove(packaging);
        await _context.SaveChangesAsync();
        return true;
    }
}
