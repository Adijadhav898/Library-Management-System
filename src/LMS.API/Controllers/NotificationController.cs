using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Authorize]
public class NotificationController : ApiControllerBase
{
    private readonly INotificationService _notifications;

    public NotificationController(INotificationService notifications)
    {
        _notifications = notifications;
    }

    /// <summary>Notifications of the logged-in user.</summary>
    [HttpGet("my")]
    public async Task<ActionResult<PaginatedResult<NotificationDto>>> My([FromQuery] NotificationPaginationRequest request, CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        return Ok(await _notifications.GetNotificationsAsync(request, userId, request.IsRead, ct));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> UnreadCount(CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        return Ok(await _notifications.GetUnreadCountAsync(userId, ct));
    }

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkRead([FromBody] MarkAsReadRequest request, CancellationToken ct)
        => await _notifications.MarkAsReadAsync(request, ct) ? Ok() : NotFound();

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        await _notifications.MarkAllAsReadAsync(userId, ct);
        return Ok();
    }

    // ---------- Staff ----------

    [HttpGet]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<PaginatedResult<NotificationDto>>> GetAll([FromQuery] NotificationPaginationRequest request, [FromQuery] Guid? userId, CancellationToken ct)
        => Ok(await _notifications.GetNotificationsAsync(request, userId, request.IsRead, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<NotificationDto>> GetById(Guid id, CancellationToken ct)
    {
        var n = await _notifications.GetByIdAsync(id, ct);
        return n == null ? NotFound() : Ok(n);
    }

    [HttpPost]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationRequest request, CancellationToken ct)
    {
        var n = await _notifications.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = n.Id }, n);
    }

    [HttpPost("send/overdue/{loanId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SendOverdue(Guid loanId, CancellationToken ct)
    {
        await _notifications.SendOverdueNotificationAsync(loanId, ct);
        return Accepted();
    }

    [HttpPost("send/due-reminder/{loanId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SendDueReminder(Guid loanId, CancellationToken ct)
    {
        await _notifications.SendDueDateReminderAsync(loanId, ct);
        return Accepted();
    }

    [HttpPost("send/fine/{fineId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SendFine(Guid fineId, CancellationToken ct)
    {
        await _notifications.SendFineNotificationAsync(fineId, ct);
        return Accepted();
    }

    [HttpPost("send/reservation-ready/{reservationId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SendReservationReady(Guid reservationId, CancellationToken ct)
    {
        await _notifications.SendReservationReadyNotificationAsync(reservationId, ct);
        return Accepted();
    }
}
