using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IMemberRepository Members { get; }
    IMemberTypeRepository MemberTypes { get; }
    ICategoryRepository Categories { get; }
    IPublisherRepository Publishers { get; }
    IAuthorRepository Authors { get; }
    IBookRepository Books { get; }
    IBookCopyRepository BookCopies { get; }
    ILoanRepository Loans { get; }
    IReservationRepository Reservations { get; }
    IFineRepository Fines { get; }
    IPaymentRepository Payments { get; }
    INotificationRepository Notifications { get; }
    ISettingRepository Settings { get; }
    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}