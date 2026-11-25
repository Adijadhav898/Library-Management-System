using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(LibraryDbContext context) : base(context) { }

    public async Task<PaginatedResult<Payment>> GetPagedAsync(PaginationRequest request, Guid? memberId = null, PaymentMode? mode = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Member)
            .Include(p => p.Fine).ThenInclude(f => f!.Loan).ThenInclude(l => l!.BookCopy).ThenInclude(bc => bc!.Book)
            .AsNoTracking();

        if (memberId.HasValue)
            query = query.Where(p => p.MemberId == memberId.Value);

        if (mode.HasValue)
            query = query.Where(p => p.Mode == mode.Value);

        if (from.HasValue)
            query = query.Where(p => p.PaidAt >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.PaidAt <= to.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.PaidAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Payment>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<Payment>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Fine).ThenInclude(f => f!.Loan).ThenInclude(l => l!.BookCopy).ThenInclude(bc => bc!.Book)
            .Where(p => p.MemberId == memberId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByFineAsync(Guid fineId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.FineId == fineId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalByPeriodAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.PaidAt >= from && p.PaidAt <= to)
            .SumAsync(p => p.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalByModeAsync(PaymentMode mode, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.Mode == mode);

        if (from.HasValue)
            query = query.Where(p => p.PaidAt >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.PaidAt <= to.Value);

        return await query.SumAsync(p => p.Amount, cancellationToken);
    }
}