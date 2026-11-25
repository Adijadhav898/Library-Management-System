using LMS.Application.DTOs.Report;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "LibrarianOrAdmin")]
[Produces("application/json")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportService reportService, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var stats = await _reportService.GetDashboardStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Get top borrowed books
    /// </summary>
    [HttpGet("top-books")]
    [ProducesResponseType(typeof(IEnumerable<TopBookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopBookDto>>> GetTopBorrowedBooks(
        [FromQuery] int count = 10,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var books = await _reportService.GetTopBorrowedBooksAsync(count, fromDate, toDate);
        return Ok(books);
    }

    /// <summary>
    /// Get top active members
    /// </summary>
    [HttpGet("top-members")]
    [ProducesResponseType(typeof(IEnumerable<TopMemberDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopMemberDto>>> GetTopActiveMembers(
        [FromQuery] int count = 10,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var members = await _reportService.GetTopActiveMembersAsync(count, fromDate, toDate);
        return Ok(members);
    }

    /// <summary>
    /// Get overdue report
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(OverdueReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OverdueReportDto>> GetOverdueReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var report = await _reportService.GetOverdueReportAsync(fromDate, toDate);
        return Ok(report);
    }

    /// <summary>
    /// Get fines report
    /// </summary>
    [HttpGet("fines")]
    [ProducesResponseType(typeof(FinesReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinesReportDto>> GetFinesReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var report = await _reportService.GetFinesReportAsync(fromDate, toDate);
        return Ok(report);
    }

    /// <summary>
    /// Get circulation report
    /// </summary>
    [HttpGet("circulation")]
    [ProducesResponseType(typeof(CirculationReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CirculationReportDto>> GetCirculationReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var report = await _reportService.GetCirculationReportAsync(fromDate, toDate);
        return Ok(report);
    }

    /// <summary>
    /// Get member activity report
    /// </summary>
    [HttpGet("member-activity")]
    [ProducesResponseType(typeof(MemberActivityReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<MemberActivityReportDto>> GetMemberActivityReport(
        [FromQuery] Guid? memberId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var report = await _reportService.GetMemberActivityReportAsync(memberId, fromDate, toDate);
        return Ok(report);
    }

    /// <summary>
    /// Get inventory report
    /// </summary>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(InventoryReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<InventoryReportDto>> GetInventoryReport()
    {
        var report = await _reportService.GetInventoryReportAsync();
        return Ok(report);
    }

    /// <summary>
    /// Get revenue report
    /// </summary>
    [HttpGet("revenue")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(RevenueReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RevenueReportDto>> GetRevenueReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var report = await _reportService.GetRevenueReportAsync(fromDate, toDate);
        return Ok(report);
    }

    /// <summary>
    /// Export report to CSV
    /// </summary>
    [HttpGet("export/{reportType}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportReport(
        string reportType,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var (content, fileName, contentType) = await _reportService.ExportReportAsync(reportType, fromDate, toDate);
            return File(content, contentType, fileName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}