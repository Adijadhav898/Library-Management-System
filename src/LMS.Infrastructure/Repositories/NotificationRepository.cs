using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(LibraryDbContext context) : base(context) { }

    public async Task<PaginatedResult<Notification>> GetPagedAsync(PaginationRequest request, Guid? userId = null, NotificationType? type = null, NotificationChannel? channel = null, bool? isRead = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(n => n.User)
            .AsNoTracking();

        if (userId.HasValue)
            query = query.Where(n => n.UserId == userId.Value);

        if (type.HasValue)
            query = query.Where(n => n.Type == type.Value);

        if (channel.HasValue)
            query = query.Where(n => n.Channel == channel.Value);

        if (isRead.HasValue)
            query = query.Where(n => n.IsRead == isRead.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Notification>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetPendingToSendAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Status == NotificationStatus.Pending)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetFailedNotificationsAsync(int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Status == NotificationStatus.Failed && n.RetryCount < maxRetries)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}