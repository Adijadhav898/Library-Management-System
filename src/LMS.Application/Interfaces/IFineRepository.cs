using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces;

public interface IFineRepository : IGenericRepository<Fine>
{
    Task<PaginatedResult<Fine>> GetPagedAsync(PaginationRequest request, Guid? memberId = null, FineStatus? status = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Fine>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Fine>> GetOutstandingByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalOutstandingByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalOutstandingAsync(CancellationToken cancellationToken = default);
    Task<PaginatedResult<FineReportInfo>> GetFineReportAsync(PaginationRequest request, CancellationToken cancellationToken = default);
}

public class FineReportInfo
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