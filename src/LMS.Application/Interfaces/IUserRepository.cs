using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    new Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    new Task<PaginatedResult<User>> GetPagedAsync(PaginationRequest request, string? search, bool? isActive, CancellationToken cancellationToken = default);
}