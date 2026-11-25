using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<PaginatedResult<Payment>> GetPagedAsync(PaginationRequest request, Guid? fineId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByFineAsync(Guid fineId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByFineAsync(Guid fineId, CancellationToken cancellationToken = default);
}