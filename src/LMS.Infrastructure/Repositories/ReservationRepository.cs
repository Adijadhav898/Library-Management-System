using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(LibraryDbContext context) : base(context) { }

    public async Task<PaginatedResult<Reservation>> GetPagedAsync(PaginationRequest request, Guid? memberId = null, Guid? bookId = null, ReservationStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(r => r.Book)
            .Include(r => r.Member)
            .Include(r => r.FulfilledByBookCopy).ThenInclude(bc => bc!.Book)
            .AsNoTracking();

        if (memberId.HasValue)
            query = query.Where(r => r.MemberId == memberId.Value);

        if (bookId.HasValue)
            query = query.Where(r => r.BookId == bookId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.ReservedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Reservation>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<Reservation>> GetActiveByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Book)
            .Where(r => r.MemberId == memberId && r.Status == ReservationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetActiveByBookAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Active)
            .OrderBy(r => r.ReservedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Reservation?> GetNextInQueueAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Member)
            .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Active)
            .OrderBy(r => r.ReservedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Status == ReservationStatus.Active && r.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}