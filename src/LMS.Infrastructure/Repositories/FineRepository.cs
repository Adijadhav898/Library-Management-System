using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class FineRepository : GenericRepository<Fine>, IFineRepository
{
    public FineRepository(LibraryDbContext context) : base(context) { }

    public async Task<PaginatedResult<Fine>> GetPagedAsync(PaginationRequest request, Guid? memberId = null, FineStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(f => f.Member)
            .Include(f => f.Loan).ThenInclude(l => l!.BookCopy).ThenInclude(bc => bc!.Book)
            .AsNoTracking();

        if (memberId.HasValue)
            query = query.Where(f => f.MemberId == memberId.Value);

        if (status.HasValue)
            query = query.Where(f => f.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Fine>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<Fine>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Loan).ThenInclude(l => l!.BookCopy).ThenInclude(bc => bc!.Book)
            .Where(f => f.MemberId == memberId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Fine>> GetUnpaidByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Loan).ThenInclude(l => l!.BookCopy).ThenInclude(bc => bc!.Book)
            .Where(f => f.MemberId == memberId && (f.Status == FineStatus.Pending || f.Status == FineStatus.Partial))
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalUnpaidByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.MemberId == memberId && (f.Status == FineStatus.Pending || f.Status == FineStatus.Partial))
            .SumAsync(f => f.Amount - f.AmountPaid, cancellationToken);
    }

    public async Task<decimal> GetTotalCollectedAsync(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(f => f.Status == FineStatus.Paid);

        if (from.HasValue)
            query = query.Where(f => f.PaidAt >= from.Value);

        if (to.HasValue)
            query = query.Where(f => f.PaidAt <= to.Value);

        return await query.SumAsync(f => f.AmountPaid, cancellationToken);
    }

    public async Task<decimal> GetTotalOutstandingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Status == FineStatus.Pending || f.Status == FineStatus.Partial)
            .SumAsync(f => f.Amount - f.AmountPaid, cancellationToken);
    }

    public async Task<PaginatedResult<FineReportInfo>> GetFineReportAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(f => f.Member).ThenInclude(m => m.User)
            .Include(f => f.Loan).ThenInclude(l => l.BookCopy).ThenInclude(bc => bc.Book)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(f => f.IncurredDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FineReportInfo
            {
                FineId = f.Id,
                MemberName = f.Member.User.FirstName + " " + f.Member.User.LastName,
                MemberNumber = f.Member.MembershipNumber,
                BookTitle = f.Loan.BookCopy.Book.Title,
                Amount = f.Amount,
                PaidAmount = f.AmountPaid,
                OutstandingAmount = f.Amount - f.AmountPaid,
                Status = f.Status,
                IncurredDate = f.IncurredDate,
                PaidDate = f.PaidAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<FineReportInfo>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}