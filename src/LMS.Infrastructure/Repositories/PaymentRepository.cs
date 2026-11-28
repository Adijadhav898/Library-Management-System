using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(LibraryDbContext context) : base(context) { }

    /// <summary>Loads the entity together with the relations the DTO mappers need (and applies the soft-delete filter).</summary>
    public override async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Fine).ThenInclude(f => f.Member).ThenInclude(m => m.User)
            .Include(x => x.ReceivedBy)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PaginatedResult<Payment>> GetPagedAsync(PaginationRequest request, Guid? fineId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Fine).ThenInclude(f => f.Member).ThenInclude(m => m.User)
            .Include(p => p.ReceivedBy)
            .AsNoTracking();

        if (fineId.HasValue)
            query = query.Where(p => p.FineId == fineId.Value);

        if (fromDate.HasValue)
            query = query.Where(p => p.PaidAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.PaidAt <= toDate.Value);

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

    public async Task<IEnumerable<Payment>> GetByFineAsync(Guid fineId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.FineId == fineId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalByFineAsync(Guid fineId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.FineId == fineId)
            .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;
    }
}
