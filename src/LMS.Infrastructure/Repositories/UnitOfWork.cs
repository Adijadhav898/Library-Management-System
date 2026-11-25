using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(LibraryDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => new UserRepository(_context);
    public IRoleRepository Roles => new RoleRepository(_context);
    public IRefreshTokenRepository RefreshTokens => new RefreshTokenRepository(_context);
    public IMemberRepository Members => new MemberRepository(_context);
    public IMemberTypeRepository MemberTypes => new MemberTypeRepository(_context);
    public ICategoryRepository Categories => new CategoryRepository(_context);
    public IPublisherRepository Publishers => new PublisherRepository(_context);
    public IAuthorRepository Authors => new AuthorRepository(_context);
    public IBookRepository Books => new BookRepository(_context);
    public IBookCopyRepository BookCopies => new BookCopyRepository(_context);
    public ILoanRepository Loans => new LoanRepository(_context);
    public IReservationRepository Reservations => new ReservationRepository(_context);
    public IFineRepository Fines => new FineRepository(_context);
    public IPaymentRepository Payments => new PaymentRepository(_context);
    public INotificationRepository Notifications => new NotificationRepository(_context);
    public ISettingRepository Settings => new SettingRepository(_context);
    public IAuditLogRepository AuditLogs => new AuditLogRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await action();
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
            await _transaction.DisposeAsync();
        await _context.DisposeAsync();
    }
}