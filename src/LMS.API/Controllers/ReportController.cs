using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Authorize(Policy = "LibrarianOrAdmin")]
public class ReportController : ApiControllerBase
{
    private readonly IReportService _reports;

    public ReportController(IReportService reports)
    {
        _reports = reports;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> Dashboard(CancellationToken ct)
        => Ok(await _reports.GetDashboardStatsAsync(ct));

    [HttpGet("top-borrowed-books")]
    public async Task<ActionResult<IEnumerable<TopBorrowedBooksDto>>> TopBorrowedBooks([FromQuery] int count = 10, CancellationToken ct = default)
        => Ok(await _reports.GetTopBorrowedBooksAsync(count, ct));

    [HttpGet("member-activity")]
    public async Task<ActionResult<IEnumerable<MemberActivityDto>>> MemberActivity([FromQuery] PaginationRequest request, CancellationToken ct)
        => Ok(await _reports.GetMemberActivityAsync(request, ct));

    [HttpGet("category-stats")]
    public async Task<ActionResult<IEnumerable<CategoryStatsDto>>> CategoryStats(CancellationToken ct)
        => Ok(await _reports.GetCategoryStatsAsync(ct));

    [HttpGet("monthly-stats")]
    public async Task<ActionResult<IEnumerable<MonthlyStatsDto>>> MonthlyStats([FromQuery] int? year, CancellationToken ct)
        => Ok(await _reports.GetMonthlyStatsAsync(year ?? DateTime.UtcNow.Year, ct));

    [HttpGet("overdue")]
    public async Task<ActionResult<PaginatedResult<OverdueReportDto>>> Overdue([FromQuery] PaginationRequest request, CancellationToken ct)
        => Ok(await _reports.GetOverdueReportAsync(request, ct));

    [HttpGet("fines")]
    public async Task<ActionResult<PaginatedResult<FineReportDto>>> Fines([FromQuery] PaginationRequest request, CancellationToken ct)
        => Ok(await _reports.GetFineReportAsync(request, ct));
}
