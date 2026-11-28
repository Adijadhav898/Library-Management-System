using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(LibraryDbContext context) : base(context) { }

    /// <summary>Loads the entity together with the relations the DTO mappers need (and applies the soft-delete filter).</summary>
    public override async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.ParentCategory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(c => c.Name == name);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<CategoryStatsInfo>> GetCategoryStatsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Select(c => new CategoryStatsInfo
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                TotalBooks = c.Books.Count,
                TotalLoans = c.Books.SelectMany(b => b.BookCopies).SelectMany(bc => bc.Loans).Count()
            })
            .ToListAsync(cancellationToken);
    }
}