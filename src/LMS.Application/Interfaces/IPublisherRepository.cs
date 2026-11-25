using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IPublisherRepository : IGenericRepository<Publisher>
{
    Task<Publisher?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    new Task<PaginatedResult<Publisher>> GetPagedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default);
}