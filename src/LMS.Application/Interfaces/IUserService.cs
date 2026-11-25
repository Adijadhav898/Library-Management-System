using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IUserService
{
    Task<PaginatedResult<UserDto>> GetAllAsync(PaginationRequest request, string? search = null, Guid? roleId = null, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default);
}