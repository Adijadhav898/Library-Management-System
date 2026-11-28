using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    public MemberRepository(LibraryDbContext context) : base(context) { }

    /// <summary>Loads the entity together with the relations the DTO mappers need (and applies the soft-delete filter).</summary>
    public override async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.User)
            .Include(x => x.MemberType)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Member?> GetByMemberNumberAsync(string memberNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .Include(m => m.MemberType)
            .FirstOrDefaultAsync(m => m.MembershipNumber == memberNumber, cancellationToken);
    }

    public async Task<Member?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .Include(m => m.MemberType)
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
    }

    public async Task<Member?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .Include(m => m.MemberType)
            .FirstOrDefaultAsync(m => m.User.Email == email, cancellationToken);
    }

    public async Task<bool> IsMemberNumberUniqueAsync(string memberNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(m => m.MembershipNumber == memberNumber);
        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(m => m.User.Email == email);
        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Member>> GetPagedAsync(PaginationRequest request, bool? isActive = null, Guid? memberTypeId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(m => m.User)
            .Include(m => m.MemberType)
            .AsNoTracking();

        if (isActive.HasValue)
            query = query.Where(m => m.IsBlocked != isActive.Value);

        if (memberTypeId.HasValue)
            query = query.Where(m => m.MemberTypeId == memberTypeId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(m =>
                m.User.FirstName.ToLower().Contains(searchLower) ||
                m.User.LastName.ToLower().Contains(searchLower) ||
                m.User.Email.ToLower().Contains(searchLower) ||
                m.MembershipNumber.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.MembershipNumber)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Member>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<int> GetActiveLoansCountAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .CountAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Active, cancellationToken);
    }

    public async Task<decimal> GetOutstandingFinesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Fines
            .Where(f => f.MemberId == memberId && (f.Status == FineStatus.Unpaid || f.Status == FineStatus.PartiallyPaid))
            .SumAsync(f => f.Amount - f.PaidAmount, cancellationToken);
    }

    public async Task<int> GetCountSinceAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(m => m.CreatedAt >= since, cancellationToken);
    }

    public async Task<PaginatedResult<MemberActivityInfo>> GetMemberActivityAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var items = await query
            .OrderBy(m => m.MembershipNumber)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MemberActivityInfo
            {
                MemberId = m.Id,
                MemberName = m.User.FirstName + " " + m.User.LastName,
                MemberNumber = m.MembershipNumber,
                TotalLoans = m.Loans.Count,
                ActiveLoans = m.Loans.Count(l => l.Status == LoanStatus.Active),
                OverdueLoans = m.Loans.Count(l => l.Status == LoanStatus.Active && l.DueDate < now),
                TotalFines = m.Fines.Sum(f => (decimal?)f.Amount) ?? 0m,
                OutstandingFines = m.Fines
                    .Where(f => f.Status == FineStatus.Unpaid || f.Status == FineStatus.PartiallyPaid)
                    .Sum(f => (decimal?)(f.Amount - f.PaidAmount)) ?? 0m
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<MemberActivityInfo>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
