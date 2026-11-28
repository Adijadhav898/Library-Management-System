using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetByMemberNumberAsync(string memberNumber, CancellationToken cancellationToken = default);
    Task<Member?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Member?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsMemberNumberUniqueAsync(string memberNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Member>> GetPagedAsync(PaginationRequest request, bool? isActive = null, Guid? memberTypeId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<int> GetActiveLoansCountAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<decimal> GetOutstandingFinesAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<int> GetCountSinceAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<PaginatedResult<MemberActivityInfo>> GetMemberActivityAsync(PaginationRequest request, CancellationToken cancellationToken = default);
}

public class MemberActivityInfo
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