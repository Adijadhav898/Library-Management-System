using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class ReportService : IReportService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookCopyRepository _bookCopyRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IFineRepository _fineRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ReportService(
        IBookRepository bookRepository,
        IBookCopyRepository bookCopyRepository,
        ILoanRepository loanRepository,
        IReservationRepository reservationRepository,
        IFineRepository fineRepository,
        IPaymentRepository paymentRepository,
        IMemberRepository memberRepository,
        ICategoryRepository categoryRepository)
    {
        _bookRepository = bookRepository;
        _bookCopyRepository = bookCopyRepository;
        _loanRepository = loanRepository;
        _reservationRepository = reservationRepository;
        _fineRepository = fineRepository;
        _paymentRepository = paymentRepository;
        _memberRepository = memberRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var totalBooks = await _bookRepository.CountAsync(cancellationToken);
        var totalMembers = await _memberRepository.CountAsync(cancellationToken);
        var activeLoans = await _loanRepository.GetActiveLoansCountAsync(cancellationToken);
        var overdueLoans = await _loanRepository.GetOverdueLoansCountAsync(cancellationToken);
        var pendingReservations = await _reservationRepository.GetPendingReservationsCountAsync(cancellationToken);

        var totalOutstandingFines = await _fineRepository.GetTotalOutstandingAsync(cancellationToken);

        var booksAddedThisMonth = await _bookRepository.GetCountSinceAsync(startOfMonth, cancellationToken);
        var newMembersThisMonth = await _memberRepository.GetCountSinceAsync(startOfMonth, cancellationToken);
        var loansThisMonth = await _loanRepository.GetCountSinceAsync(startOfMonth, cancellationToken);
        var returnsThisMonth = await _loanRepository.GetReturnsCountSinceAsync(startOfMonth, cancellationToken);

        return new DashboardStatsDto
        {
            TotalBooks = totalBooks,
            TotalMembers = totalMembers,
            ActiveLoans = activeLoans,
            OverdueLoans = overdueLoans,
            PendingReservations = pendingReservations,
            TotalOutstandingFines = totalOutstandingFines,
            BooksAddedThisMonth = booksAddedThisMonth,
            NewMembersThisMonth = newMembersThisMonth,
            LoansThisMonth = loansThisMonth,
            ReturnsThisMonth = returnsThisMonth
        };
    }

    public async Task<IEnumerable<TopBorrowedBooksDto>> GetTopBorrowedBooksAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var topBooks = await _loanRepository.GetTopBorrowedBooksAsync(count, cancellationToken);

        return topBooks.Select(tb => new TopBorrowedBooksDto
        {
            BookId = tb.BookId,
            Title = tb.Title,
            ISBN = tb.ISBN,
            BorrowCount = tb.BorrowCount
        }).ToList();
    }

    public async Task<IEnumerable<MemberActivityDto>> GetMemberActivityAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var memberActivities = await _memberRepository.GetMemberActivityAsync(request, cancellationToken);

        return memberActivities.Items.Select(ma => new MemberActivityDto
        {
            MemberId = ma.MemberId,
            MemberName = ma.MemberName,
            MemberNumber = ma.MemberNumber,
            TotalLoans = ma.TotalLoans,
            ActiveLoans = ma.ActiveLoans,
            OverdueLoans = ma.OverdueLoans,
            TotalFines = ma.TotalFines,
            OutstandingFines = ma.OutstandingFines
        }).ToList();
    }

    public async Task<IEnumerable<CategoryStatsDto>> GetCategoryStatsAsync(CancellationToken cancellationToken = default)
    {
        var categoryStats = await _categoryRepository.GetCategoryStatsAsync(cancellationToken);

        return categoryStats.Select(cs => new CategoryStatsDto
        {
            CategoryId = cs.CategoryId,
            CategoryName = cs.CategoryName,
            TotalBooks = cs.TotalBooks,
            TotalLoans = cs.TotalLoans
        }).ToList();
    }

    public async Task<IEnumerable<MonthlyStatsDto>> GetMonthlyStatsAsync(int year, CancellationToken cancellationToken = default)
    {
        var monthlyStats = await _loanRepository.GetMonthlyStatsAsync(year, cancellationToken);

        return monthlyStats.Select(ms => new MonthlyStatsDto
        {
            Year = ms.Year,
            Month = ms.Month,
            NewBooks = ms.NewBooks,
            NewMembers = ms.NewMembers,
            TotalLoans = ms.TotalLoans,
            TotalReturns = ms.TotalReturns,
            TotalFinesCollected = ms.TotalFinesCollected
        }).ToList();
    }

    public async Task<PaginatedResult<OverdueReportDto>> GetOverdueReportAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var (overdueLoans, totalCount) = await _loanRepository.GetOverdueReportAsync(request, cancellationToken);

        var overdueReportDtos = overdueLoans.Select(ol => new OverdueReportDto
        {
            LoanId = ol.LoanId,
            BookTitle = ol.BookTitle,
            CopyNumber = ol.CopyNumber,
            MemberName = ol.MemberName,
            MemberNumber = ol.MemberNumber,
            IssueDate = ol.IssueDate,
            DueDate = ol.DueDate,
            DaysOverdue = ol.DaysOverdue,
            EstimatedFine = ol.EstimatedFine
        }).ToList();

        return new PaginatedResult<OverdueReportDto>
        {
            Items = overdueReportDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PaginatedResult<FineReportDto>> GetFineReportAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var (fines, totalCount) = await _fineRepository.GetFineReportAsync(request, cancellationToken);

        var fineReportDtos = fines.Select(f => new FineReportDto
        {
            FineId = f.FineId,
            MemberName = f.MemberName,
            MemberNumber = f.MemberNumber,
            BookTitle = f.BookTitle,
            Amount = f.Amount,
            PaidAmount = f.PaidAmount,
            OutstandingAmount = f.OutstandingAmount,
            Status = f.Status,
            IncurredDate = f.IncurredDate,
            PaidDate = f.PaidDate
        }).ToList();

        return new PaginatedResult<FineReportDto>
        {
            Items = fineReportDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}