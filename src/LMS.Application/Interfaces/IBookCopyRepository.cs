using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces;

public interface IBookCopyRepository : IGenericRepository<BookCopy>
{
    Task<IEnumerable<BookCopy>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<BookCopy?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<bool> IsBarcodeUniqueAsync(string barcode, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookCopy>> GetAvailableByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<BookCopy>> GetPagedAsync(PaginationRequest request, Guid? bookId = null, CopyStatus? status = null, CancellationToken cancellationToken = default);
}