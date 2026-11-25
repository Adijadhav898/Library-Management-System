using LMS.Application.DTOs.Notification;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all notifications for current user with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<NotificationDto>>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isRead = null)
    {
        var userId = GetCurrentUserId();
        var result = await _notificationService.GetUserNotificationsAsync(userId, page, pageSize, isRead);
        return Ok(result);
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> GetNotification(Guid id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
        {
            return NotFound(new { message = "Notification not found" });
        }
        return Ok(notification);
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> MarkAsRead(Guid id)
    {
        var notification = await _notificationService.MarkAsReadAsync(id);
        if (notification == null)
        {
            return NotFound(new { message = "Notification not found" });
        }
        return Ok(notification);
    }

    /// <summary>
    /// Mark all notifications as read for current user
    /// </summary>
    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetCurrentUserId();
        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(new { message = "All notifications marked as read" });
    }

    /// <summary>
    /// Get unread notification count for current user
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    /// <summary>
    /// Send notification to specific user (Admin/Librarian only)
    /// </summary>
    [HttpPost("send")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationDto>> SendNotification([FromBody] SendNotificationRequestDto request)
    {
        var notification = await _notificationService.SendNotificationAsync(request);
        return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
    }

    /// <summary>
    /// Send bulk notification to multiple users (Admin only)
    /// </summary>
    [HttpPost("send-bulk")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> SendBulkNotification([FromBody] SendBulkNotificationRequestDto request)
    {
        var notifications = await _notificationService.SendBulkNotificationAsync(request);
        return CreatedAtAction(nameof(GetNotifications), new { }, notifications);
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        var result = await _notificationService.DeleteNotificationAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Notification not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Trigger overdue notifications (Admin only)
    /// </summary>
    [HttpPost("trigger-overdue")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TriggerOverdueNotifications()
    {
        await _notificationService.SendOverdueNotificationsAsync();
        return Ok(new { message = "Overdue notifications triggered" });
    }

    /// <summary>
    /// Trigger reservation ready notifications (Admin only)
    /// </summary>
    [HttpPost("trigger-reservation-ready")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TriggerReservationReadyNotifications()
    {
        await _notificationService.SendReservationReadyNotificationsAsync();
        return Ok(new { message = "Reservation ready notifications triggered" });
    }

    /// <summary>
    /// Trigger fine notifications (Admin only)
    /// </summary>
    [HttpPost("trigger-fines")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TriggerFineNotifications()
    {
        await _notificationService.SendFineNotificationsAsync();
        return Ok(new { message = "Fine notifications triggered" });
    }

    /// <summary>
    /// Trigger due date reminders (Admin only)
    /// </summary>
    [HttpPost("trigger-due-date-reminders")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TriggerDueDateReminders()
    {
        await _notificationService.SendDueDateRemindersAsync();
        return Ok(new { message = "Due date reminders triggered" });
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }
}