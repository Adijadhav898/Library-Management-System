namespace LMS.Application.DTOs;

using LMS.Domain.Enums;

public class DashboardStatsDto
{
    public int TotalBooks { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int PendingReservations { get; set; }
    public decimal TotalOutstandingFines { get; set; }
    public int BooksAddedThisMonth { get; set; }
    public int NewMembersThisMonth { get; set; }
    public int LoansThisMonth { get; set; }
    public int ReturnsThisMonth { get; set; }
}

public class TopBorrowedBooksDto
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int BorrowCount { get; set; }
}

public class MemberActivityDto
{
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MemberNumber { get; set; } = string.Empty;
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public decimal TotalFines { get; set; }
    public decimal OutstandingFines { get; set; }
}

public class CategoryStatsDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TotalBooks { get; set; }
    public int TotalLoans { get; set; }
}

public class MonthlyStatsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int NewBooks { get; set; }
    public int NewMembers { get; set; }
    public int TotalLoans { get; set; }
    public int TotalReturns { get; set; }
    public decimal TotalFinesCollected { get; set; }
}

public class OverdueReportDto
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

public class FineReportDto
{
    public Guid FineId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MemberNumber { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public FineStatus Status { get; set; }
    public DateTime IncurredDate { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class ReportFilterRequest : PaginationRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? MemberId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class MonthlyStatsRequest
{
    public int Year { get; set; }
}

public class TopBorrowedBooksRequest
{
    public int Count { get; set; }
}