using LMS.Application.DTOs;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces;

public interface ICatalogService
{
    // Categories
    Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    // Publishers
    Task<PaginatedResult<PublisherDto>> GetPublishersAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default);
    Task<PublisherDto?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PublisherDto> CreatePublisherAsync(CreatePublisherRequest request, CancellationToken cancellationToken = default);
    Task<PublisherDto?> UpdatePublisherAsync(Guid id, UpdatePublisherRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeletePublisherAsync(Guid id, CancellationToken cancellationToken = default);

    // Authors
    Task<PaginatedResult<AuthorDto>> GetAuthorsAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default);
    Task<AuthorDto?> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AuthorDto> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<AuthorDto?> UpdateAuthorAsync(Guid id, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAuthorAsync(Guid id, CancellationToken cancellationToken = default);

    // Books
    Task<PaginatedResult<BookDto>> GetBooksAsync(PaginationRequest request, Guid? categoryId = null, Guid? publisherId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookDto> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<BookDto?> UpdateBookAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookSummaryDto>> SearchBooksAsync(string query, int maxResults = 10, CancellationToken cancellationToken = default);

    // Book Copies
    Task<PaginatedResult<BookCopyDto>> GetBookCopiesAsync(PaginationRequest request, Guid? bookId = null, CopyStatus? status = null, CancellationToken cancellationToken = default);
    Task<BookCopyDto?> GetBookCopyByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookCopyDto> CreateBookCopyAsync(CreateBookCopyRequest request, CancellationToken cancellationToken = default);
    Task<BookCopyDto?> UpdateBookCopyAsync(Guid id, UpdateBookCopyRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteBookCopyAsync(Guid id, CancellationToken cancellationToken = default);
}