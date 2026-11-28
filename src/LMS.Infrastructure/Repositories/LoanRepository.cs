using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class LoanRepository : GenericRepository<Loan>, ILoanRepository
{
    public LoanRepository(LibraryDbContext context) : base(context) { }

    public async Task<PaginatedResult<Loan>> GetPagedAsync(PaginationRequest request, Guid? memberId = null, Guid? bookCopyId = null, LoanStatus? status = null, bool? isOverdue = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(l => l.BookCopy).ThenInclude(bc => bc.Book)
            .Include(l => l.Member)
            .Include(l => l.IssuedBy)
            .Include(l => l.ReturnedByUser)
            .AsNoTracking();

        if (memberId.HasValue)
            query = query.Where(l => l.MemberId == memberId.Value);

        if (bookCopyId.HasValue)
            query = query.Where(l => l.BookCopyId == bookCopyId.Value);

        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        if (isOverdue.HasValue && isOverdue.Value)
            query = query.Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.IssueDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Loan>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(l => l.BookCopy).ThenInclude(bc => bc.Book)
            .Where(l => l.MemberId == memberId && l.Status == LoanStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(l => l.BookCopy).ThenInclude(bc => bc.Book)
            .Include(l => l.Member)
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loan?> GetActiveLoanByBookCopyAsync(Guid bookCopyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.BookCopyId == bookCopyId && l.Status == LoanStatus.Active, cancellationToken);
    }

    public async Task<int> GetActiveLoansCountByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Active, cancellationToken);
    }

    public async Task<int> GetActiveLoansCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(l => l.Status == LoanStatus.Active, cancellationToken);
    }

    public async Task<int> GetOverdueLoansCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow, cancellationToken);
    }

    public async Task<int> GetCountSinceAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(l => l.IssueDate >= since, cancellationToken);
    }

    public async Task<int> GetReturnsCountSinceAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(l => l.Status == LoanStatus.Returned && l.ReturnDate >= since, cancellationToken);
    }

    public async Task<IEnumerable<TopBorrowedBookInfo>> GetTopBorrowedBooksAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.BookCopy != null && l.BookCopy.Book != null)
            .GroupBy(l => new { l.BookCopy.BookId, l.BookCopy.Book.Title, l.BookCopy.Book.ISBN })
            .Select(g => new TopBorrowedBookInfo
            {
                BookId = g.Key.BookId,
                Title = g.Key.Title,
                ISBN = g.Key.ISBN,
                BorrowCount = g.Count()
            })
            .OrderByDescending(x => x.BorrowCount)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<OverdueReportInfo>> GetOverdueReportAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(l => l.BookCopy).ThenInclude(bc => bc.Book)
            .Include(l => l.Member).ThenInclude(m => m.User)
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(l => l.DueDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new OverdueReportInfo
            {
                LoanId = l.Id,
                BookTitle = l.BookCopy.Book.Title,
                CopyNumber = l.BookCopy.CopyNumber,
                MemberName = l.Member.User.FirstName + " " + l.Member.User.LastName,
                MemberNumber = l.Member.MembershipNumber,
                IssueDate = l.IssueDate,
                DueDate = l.DueDate,
                DaysOverdue = (DateTime.UtcNow.Date - l.DueDate.Date).Days,
                EstimatedFine = (DateTime.UtcNow.Date - l.DueDate.Date).Days * 1.00m // Default fine rate
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<OverdueReportInfo>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<MonthlyStatsInfo>> GetMonthlyStatsAsync(int year, CancellationToken cancellationToken = default)
    {
        var startOfYear = new DateTime(year, 1, 1);
        var endOfYear = new DateTime(year, 12, 31, 23, 59, 59);

        var loansInYear = await _dbSet
            .Where(l => l.IssueDate >= startOfYear && l.IssueDate <= endOfYear)
            .ToListAsync(cancellationToken);

        var returnsInYear = await _dbSet
            .Where(l => l.Status == LoanStatus.Returned && l.ReturnDate >= startOfYear && l.ReturnDate <= endOfYear)
            .ToListAsync(cancellationToken);

        var finesInYear = await _context.Payments
            .Where(p => p.PaidAt >= startOfYear && p.PaidAt <= endOfYear)
            .ToListAsync(cancellationToken);

        var newBooksInYear = await _context.Books
            .Where(b => b.CreatedAt >= startOfYear && b.CreatedAt <= endOfYear)
            .ToListAsync(cancellationToken);

        var newMembersInYear = await _context.Members
            .Where(m => m.CreatedAt >= startOfYear && m.CreatedAt <= endOfYear)
            .ToListAsync(cancellationToken);

        var monthlyStats = new List<MonthlyStatsInfo>();

        for (int month = 1; month <= 12; month++)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            var loansThisMonth = loansInYear.Count(l => l.IssueDate >= monthStart && l.IssueDate <= monthEnd);
            var returnsThisMonth = returnsInYear.Count(l => l.ReturnDate.HasValue && l.ReturnDate >= monthStart && l.ReturnDate <= monthEnd);
            var finesThisMonth = finesInYear.Where(p => p.PaidAt >= monthStart && p.PaidAt <= monthEnd).Sum(p => p.Amount);
            var newBooksThisMonth = newBooksInYear.Count(b => b.CreatedAt >= monthStart && b.CreatedAt <= monthEnd);
            var newMembersThisMonth = newMembersInYear.Count(m => m.CreatedAt >= monthStart && m.CreatedAt <= monthEnd);

            monthlyStats.Add(new MonthlyStatsInfo
            {
                Year = year,
                Month = month,
                NewBooks = newBooksThisMonth,
                NewMembers = newMembersThisMonth,
                TotalLoans = loansThisMonth,
                TotalReturns = returnsThisMonth,
                TotalFinesCollected = finesThisMonth
            });
        }

        return monthlyStats;
    }
}