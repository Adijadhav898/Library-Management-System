using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(LibraryDbContext context) : base(context) { }

    public async Task<PaginatedResult<AuditLog>> GetPagedAsync(PaginationRequest request, string? entityName = null, string? entityId = null, Guid? userId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.User)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(a => a.EntityName == entityName);

        if (!string.IsNullOrWhiteSpace(entityId) && Guid.TryParse(entityId, out var entityGuid))
            query = query.Where(a => a.EntityId == entityGuid);

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId.Value);

        if (fromDate.HasValue)
            query = query.Where(a => a.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.CreatedAt <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AuditLog>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(entityId, out var entityGuid))
            return Enumerable.Empty<AuditLog>();

        return await _dbSet
            .Where(a => a.EntityName == entityName && a.EntityId == entityGuid)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
