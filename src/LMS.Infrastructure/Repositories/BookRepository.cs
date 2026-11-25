using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context) { }

    public async Task<Book?> GetByISBNAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.ISBN == isbn, cancellationToken);
    }

    public async Task<bool> IsISBNUniqueAsync(string isbn, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(b => b.ISBN == isbn);
        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Book>> GetPagedAsync(PaginationRequest request, Guid? categoryId = null, Guid? publisherId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .AsNoTracking();

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        if (publisherId.HasValue)
            query = query.Where(b => b.PublisherId == publisherId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(b =>
                b.Title.ToLower().Contains(searchLower) ||
                b.ISBN.ToLower().Contains(searchLower) ||
                b.BookAuthors.Any(ba => ba.Author.FirstName.ToLower().Contains(searchLower) || ba.Author.LastName.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Book>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<Book>> GetByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAvailableCopiesCountAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _context.BookCopies
            .CountAsync(bc => bc.BookId == bookId && bc.Status == Domain.Enums.CopyStatus.Available, cancellationToken);
    }

    public async Task<IEnumerable<BookCopy>> GetAvailableCopiesAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _context.BookCopies
            .Where(bc => bc.BookId == bookId && bc.Status == Domain.Enums.CopyStatus.Available)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> SearchAsync(string query, int maxResults, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<Book>();

        var searchLower = query.ToLower();
        return await _dbSet
            .Include(b => b.Category)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCopies)
            .AsNoTracking()
            .Where(b =>
                b.Title.ToLower().Contains(searchLower) ||
                b.ISBN.ToLower().Contains(searchLower) ||
                b.BookAuthors.Any(ba => ba.Author.FirstName.ToLower().Contains(searchLower) || ba.Author.LastName.ToLower().Contains(searchLower)))
            .Take(maxResults)
            .ToListAsync(cancellationToken);
    }
}