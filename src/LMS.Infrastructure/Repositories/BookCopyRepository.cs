using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class BookCopyRepository : GenericRepository<BookCopy>, IBookCopyRepository
{
    public BookCopyRepository(LibraryDbContext context) : base(context) { }

    /// <summary>Loads the entity together with the relations the DTO mappers need (and applies the soft-delete filter).</summary>
    public override async Task<BookCopy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Book)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<BookCopy>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(bc => bc.BookId == bookId).ToListAsync(cancellationToken);
    }

    public async Task<BookCopy?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(bc => bc.Barcode == barcode, cancellationToken);
    }

    public async Task<bool> IsBarcodeUniqueAsync(string barcode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(bc => bc.Barcode == barcode);
        if (excludeId.HasValue)
            query = query.Where(bc => bc.Id != excludeId.Value);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<BookCopy>> GetAvailableByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(bc => bc.BookId == bookId && bc.Status == CopyStatus.Available)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<BookCopy>> GetPagedAsync(PaginationRequest request, Guid? bookId = null, CopyStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (bookId.HasValue)
            query = query.Where(bc => bc.BookId == bookId.Value);

        if (status.HasValue)
            query = query.Where(bc => bc.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(bc => bc.Book)
            .OrderBy(bc => bc.Barcode)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<BookCopy>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}