using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PaginatedResult<T>> GetPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedResult<T>> GetPagedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}