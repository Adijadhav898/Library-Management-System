using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface INotificationService
{
    Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(PaginationRequest request, Guid? userId = null, bool? isRead = null, CancellationToken cancellationToken = default);
    Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationDto> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(MarkAsReadRequest request, CancellationToken cancellationToken = default);
    Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

    Task SendOverdueNotificationAsync(Guid loanId, CancellationToken cancellationToken = default);
    Task SendReservationReadyNotificationAsync(Guid reservationId, CancellationToken cancellationToken = default);
    Task SendFineNotificationAsync(Guid fineId, CancellationToken cancellationToken = default);
    Task SendDueDateReminderAsync(Guid loanId, CancellationToken cancellationToken = default);
}

public interface ISettingService
{
    Task<PaginatedResult<SettingDto>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<SettingDto?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<SettingDto> UpdateAsync(string key, UpdateSettingRequest request, CancellationToken cancellationToken = default);
    Task<T?> GetValueAsync<T>(string key, T? defaultValue = default, CancellationToken cancellationToken = default) where T : struct;
    Task<string?> GetValueAsync(string key, string? defaultValue = null, CancellationToken cancellationToken = default);
}