using LMS.Application.DTOs;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces;

public interface ICirculationService
{
    // Loans
    Task<PaginatedResult<LoanDto>> GetLoansAsync(PaginationRequest request, Guid? memberId = null, Guid? bookCopyId = null, LoanStatus? status = null, bool? isOverdue = null, CancellationToken cancellationToken = default);
    Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LoanDto> IssueBookAsync(CreateLoanRequest request, Guid issuedById, CancellationToken cancellationToken = default);
    Task<LoanDto?> RenewLoanAsync(RenewLoanRequest request, Guid renewedById, CancellationToken cancellationToken = default);
    Task<LoanDto?> ReturnBookAsync(ReturnLoanRequest request, Guid returnedById, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task ProcessOverdueLoansAsync(CancellationToken cancellationToken = default);

    // Reservations
    Task<PaginatedResult<ReservationDto>> GetReservationsAsync(PaginationRequest request, Guid? memberId = null, Guid? bookId = null, ReservationStatus? status = null, CancellationToken cancellationToken = default);
    Task<ReservationDto?> GetReservationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReservationDto> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
    Task<bool> CancelReservationAsync(CancelReservationRequest request, CancellationToken cancellationToken = default);
    Task ProcessExpiredReservationsAsync(CancellationToken cancellationToken = default);

    // Fines
    Task<PaginatedResult<FineDto>> GetFinesAsync(PaginationRequest request, Guid? memberId = null, FineStatus? status = null, CancellationToken cancellationToken = default);
    Task<FineDto?> GetFineByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FineDto> CreateFineAsync(CreateFineRequest request, CancellationToken cancellationToken = default);
    Task<FineDto?> UpdateFineStatusAsync(Guid fineId, FineStatus status, CancellationToken cancellationToken = default);

    // Payments
    Task<PaginatedResult<PaymentDto>> GetPaymentsAsync(PaginationRequest request, Guid? fineId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<PaymentDto> RecordPaymentAsync(CreatePaymentRequest request, Guid receivedById, CancellationToken cancellationToken = default);
}