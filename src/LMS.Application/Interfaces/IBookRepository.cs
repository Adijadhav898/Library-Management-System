using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetByISBNAsync(string isbn, CancellationToken cancellationToken = default);
    Task<bool> IsISBNUniqueAsync(string isbn, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Book>> GetPagedAsync(PaginationRequest request, Guid? categoryId = null, Guid? publisherId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<int> GetAvailableCopiesCountAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookCopy>> GetAvailableCopiesAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<int> GetCountSinceAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> SearchAsync(string query, int maxResults, CancellationToken cancellationToken = default);
}