using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<PaginatedResult<Reservation>> GetPagedAsync(PaginationRequest request, Guid? memberId = null, Guid? bookId = null, ReservationStatus? status = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetActiveByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetActiveByBookAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<Reservation?> GetNextInQueueAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default);
    Task<int> GetPendingReservationsCountAsync(CancellationToken cancellationToken = default);
}