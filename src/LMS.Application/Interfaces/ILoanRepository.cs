using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<PaginatedResult<Loan>> GetPagedAsync(PaginationRequest request, Guid? memberId = null, Guid? bookCopyId = null, LoanStatus? status = null, bool? isOverdue = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task<Loan?> GetActiveLoanByBookCopyAsync(Guid bookCopyId, CancellationToken cancellationToken = default);
    Task<int> GetActiveLoansCountByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<int> GetActiveLoansCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetOverdueLoansCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountSinceAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<int> GetReturnsCountSinceAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<IEnumerable<TopBorrowedBookInfo>> GetTopBorrowedBooksAsync(int count, CancellationToken cancellationToken = default);
    Task<PaginatedResult<OverdueReportInfo>> GetOverdueReportAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<MonthlyStatsInfo>> GetMonthlyStatsAsync(int year, CancellationToken cancellationToken = default);
}

public class TopBorrowedBookInfo
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int BorrowCount { get; set; }
}

public class OverdueReportInfo
{
    public Guid LoanId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string CopyNumber { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string MemberNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public decimal EstimatedFine { get; set; }
}

public class MonthlyStatsInfo
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int NewBooks { get; set; }
    public int NewMembers { get; set; }
    public int TotalLoans { get; set; }
    public int TotalReturns { get; set; }
    public decimal TotalFinesCollected { get; set; }
}