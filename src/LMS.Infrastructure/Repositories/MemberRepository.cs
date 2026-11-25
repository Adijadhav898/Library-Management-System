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

    public async Task<Member?> GetByMemberNumberAsync(string memberNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.MemberType)
            .FirstOrDefaultAsync(m => m.MemberNumber == memberNumber, cancellationToken);
    }

    public async Task<Member?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.MemberType)
            .FirstOrDefaultAsync(m => m.Email == email, cancellationToken);
    }

    public async Task<bool> IsMemberNumberUniqueAsync(string memberNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(m => m.MemberNumber == memberNumber);
        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(m => m.Email == email);
        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Member>> GetPagedAsync(PaginationRequest request, bool? isActive = null, Guid? memberTypeId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(m => m.MemberType).AsNoTracking();

        if (isActive.HasValue)
            query = query.Where(m => m.IsActive == isActive.Value);

        if (memberTypeId.HasValue)
            query = query.Where(m => m.MemberTypeId == memberTypeId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(m =>
                m.FirstName.ToLower().Contains(searchLower) ||
                m.LastName.ToLower().Contains(searchLower) ||
                m.Email.ToLower().Contains(searchLower) ||
                m.MemberNumber.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
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
            .Where(f => f.MemberId == memberId && f.Status != FineStatus.Paid)
            .SumAsync(f => f.Amount - f.Payments.Sum(p => p.Amount), cancellationToken);
    }

    public async Task<int> GetCountSinceAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(m => m.CreatedAt >= since, cancellationToken);
    }

    public async Task<PaginatedResult<MemberActivityInfo>> GetMemberActivityAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(m => m.User)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MemberActivityInfo
            {
                MemberId = m.Id,
                MemberName = m.User.FirstName + " " + m.User.LastName,
                MemberNumber = m.MemberNumber,
                TotalLoans = _context.Loans.Count(l => l.MemberId == m.Id),
                ActiveLoans = _context.Loans.Count(l => l.MemberId == m.Id && l.Status == LoanStatus.Active),
                OverdueLoans = _context.Loans.Count(l => l.MemberId == m.Id && l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow),
                TotalFines = _context.Fines.Where(f => f.MemberId == m.Id).Sum(f => f.Amount),
                OutstandingFines = _context.Fines.Where(f => f.MemberId == m.Id && f.Status != FineStatus.Paid).Sum(f => f.Amount - f.AmountPaid)
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