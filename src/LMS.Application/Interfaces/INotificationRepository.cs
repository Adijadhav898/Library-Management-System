using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<PaginatedResult<Notification>> GetPagedAsync(PaginationRequest request, Guid? userId = null, bool? isRead = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
}