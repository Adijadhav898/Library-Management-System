using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IMemberService
{
    Task<PaginatedResult<MemberDto>> GetAllAsync(PaginationRequest request, bool? isActive = null, Guid? memberTypeId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<MemberDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MemberDto?> GetByMemberNumberAsync(string memberNumber, CancellationToken cancellationToken = default);
    Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default);
    Task<MemberDto?> UpdateAsync(Guid id, UpdateMemberRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedResult<MemberTypeDto>> GetMemberTypesAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<MemberTypeDto?> GetMemberTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MemberTypeDto> CreateMemberTypeAsync(CreateMemberTypeRequest request, CancellationToken cancellationToken = default);
    Task<MemberTypeDto?> UpdateMemberTypeAsync(Guid id, UpdateMemberTypeRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteMemberTypeAsync(Guid id, CancellationToken cancellationToken = default);
}