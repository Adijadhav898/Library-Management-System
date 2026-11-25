using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IReportService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TopBorrowedBooksDto>> GetTopBorrowedBooksAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<MemberActivityDto>> GetMemberActivityAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryStatsDto>> GetCategoryStatsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MonthlyStatsDto>> GetMonthlyStatsAsync(int year, CancellationToken cancellationToken = default);
    Task<PaginatedResult<OverdueReportDto>> GetOverdueReportAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedResult<FineReportDto>> GetFineReportAsync(PaginationRequest request, CancellationToken cancellationToken = default);
}