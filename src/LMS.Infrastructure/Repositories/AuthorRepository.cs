using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(LibraryDbContext context) : base(context) { }

    public async Task<IEnumerable<Author>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var searchLower = name.ToLower();
        return await _dbSet
            .Where(a => a.FirstName.ToLower().Contains(searchLower) || a.LastName.ToLower().Contains(searchLower))
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Author>> GetPagedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(a => a.FirstName.ToLower().Contains(searchLower) || a.LastName.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Author>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}