using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IFineRepository _fineRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IEmailService _emailService;

    public NotificationService(
        INotificationRepository notificationRepository,
        ILoanRepository loanRepository,
        IReservationRepository reservationRepository,
        IFineRepository fineRepository,
        IMemberRepository memberRepository,
        IEmailService emailService)
    {
        _notificationRepository = notificationRepository;
        _loanRepository = loanRepository;
        _reservationRepository = reservationRepository;
        _fineRepository = fineRepository;
        _memberRepository = memberRepository;
        _emailService = emailService;
    }

    public async Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(PaginationRequest request, Guid? userId = null, bool? isRead = null, CancellationToken cancellationToken = default)
    {
        var (notifications, totalCount) = await _notificationRepository.GetPagedAsync(request, userId, isRead, cancellationToken);

        var notificationDtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        }).ToList();

        return new PaginatedResult<NotificationDto>
        {
            Items = notificationDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification == null)
            return null;

        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);

        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }

    public async Task<bool> MarkAsReadAsync(MarkAsReadRequest request, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification == null || notification.IsRead)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _notificationRepository.GetUnreadByUserAsync(userId, cancellationToken);
        if (!unreadNotifications.Any())
            return true;

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _notificationRepository.UpdateRangeAsync(unreadNotifications, cancellationToken);
        return true;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
    }

    public async Task SendOverdueNotificationAsync(Guid loanId, CancellationToken cancellationToken = default)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId, cancellationToken);
        if (loan == null)
            return;

        var member = await _memberRepository.GetByIdAsync(loan.MemberId, cancellationToken);
        if (member == null)
            return;

        var daysOverdue = (DateTime.UtcNow.Date - loan.DueDate.Date).Days;
        var estimatedFine = daysOverdue * loan.BookCopy?.Book?.MemberType?.MaxBooksAllowed ?? 0;

        var title = "Overdue Book Reminder";
        var message = $"Your book '{loan.BookCopy?.Book?.Title}' (Copy: {loan.BookCopy?.CopyNumber}) was due on {loan.DueDate:yyyy-MM-dd}. " +
                      $"It is now {daysOverdue} day(s) overdue. Estimated fine: {estimatedFine:C}. " +
                      $"Please return the book as soon as possible.";

        await CreateAsync(new CreateNotificationRequest
        {
            UserId = member.UserId,
            Title = title,
            Message = message
        }, cancellationToken);

        await _emailService.SendAsync(member.User?.Email ?? string.Empty, title, message, cancellationToken);
    }

    public async Task SendReservationReadyNotificationAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
        if (reservation == null)
            return;

        var member = await _memberRepository.GetByIdAsync(reservation.MemberId, cancellationToken);
        if (member == null)
            return;

        var title = "Reserved Book Ready for Pickup";
        var message = $"Your reserved book '{reservation.BookCopy?.Book?.Title}' is now available for pickup. " +
                      $"Please collect it from the library within 3 days (until {reservation.ExpiryDate:yyyy-MM-dd}).";

        await CreateAsync(new CreateNotificationRequest
        {
            UserId = member.UserId,
            Title = title,
            Message = message
        }, cancellationToken);

        await _emailService.SendAsync(member.User?.Email ?? string.Empty, title, message, cancellationToken);
    }

    public async Task SendFineNotificationAsync(Guid fineId, CancellationToken cancellationToken = default)
    {
        var fine = await _fineRepository.GetByIdAsync(fineId, cancellationToken);
        if (fine == null)
            return;

        var loan = await _loanRepository.GetByIdAsync(fine.LoanId, cancellationToken);
        var member = await _memberRepository.GetByIdAsync(fine.MemberId, cancellationToken);
        if (member == null)
            return;

        var title = "Fine Incurred";
        var message = $"A fine of {fine.Amount:C} has been applied to your account for " +
                      $"'{loan?.BookCopy?.Book?.Title}' (Copy: {loan?.BookCopy?.CopyNumber}). " +
                      $"Reason: {fine.Reason}. Please pay at your earliest convenience.";

        await CreateAsync(new CreateNotificationRequest
        {
            UserId = member.UserId,
            Title = title,
            Message = message
        }, cancellationToken);

        await _emailService.SendAsync(member.User?.Email ?? string.Empty, title, message, cancellationToken);
    }

    public async Task SendDueDateReminderAsync(Guid loanId, CancellationToken cancellationToken = default)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId, cancellationToken);
        if (loan == null)
            return;

        var member = await _memberRepository.GetByIdAsync(loan.MemberId, cancellationToken);
        if (member == null)
            return;

        var daysUntilDue = (loan.DueDate.Date - DateTime.UtcNow.Date).Days;
        var title = "Book Due Date Reminder";
        var message = $"Your book '{loan.BookCopy?.Book?.Title}' (Copy: {loan.BookCopy?.CopyNumber}) is due on {loan.DueDate:yyyy-MM-dd} " +
                      $"({daysUntilDue} day(s) from now). Please return or renew it before the due date to avoid fines.";

        await CreateAsync(new CreateNotificationRequest
        {
            UserId = member.UserId,
            Title = title,
            Message = message
        }, cancellationToken);

        await _emailService.SendAsync(member.User?.Email ?? string.Empty, title, message, cancellationToken);
    }
}