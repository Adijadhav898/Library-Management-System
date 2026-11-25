namespace LMS.Application.DTOs;

using LMS.Domain.Enums;

public class NotificationDto : BaseDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string TypeName => Type.ToString();
    public NotificationPriority Priority { get; set; }
    public string PriorityName => Priority.ToString();
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityName { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}

public class MarkAsReadRequest
{
    public Guid NotificationId { get; set; }
}

public class SettingDto : BaseDto
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Description { get; set; }
}

public class UpdateSettingRequest
{
    public string? Value { get; set; }
    public string? Description { get; set; }
}

public class NotificationPaginationRequest : PaginationRequest
{
    public bool? IsRead { get; set; }
    public NotificationType? Type { get; set; }
}