using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IAuthorRepository : IGenericRepository<Author>
{
    Task<IEnumerable<Author>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    new Task<PaginatedResult<Author>> GetPagedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default);
}