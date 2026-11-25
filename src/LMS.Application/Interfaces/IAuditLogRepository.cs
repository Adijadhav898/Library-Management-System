using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task<PaginatedResult<AuditLog>> GetPagedAsync(PaginationRequest request, string? entityName = null, string? entityId = null, Guid? userId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default);
}