using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

/// <summary>Loans, reservations, fines and payments. Staff only.</summary>
[Authorize(Policy = "LibrarianOrAdmin")]
public class CirculationController : ApiControllerBase
{
    private readonly ICirculationService _circulation;

    public CirculationController(ICirculationService circulation)
    {
        _circulation = circulation;
    }

    // ---------- Loans ----------

    [HttpGet("loans")]
    public async Task<ActionResult<PaginatedResult<LoanDto>>> GetLoans([FromQuery] CirculationPaginationRequest request, CancellationToken ct)
        => Ok(await _circulation.GetLoansAsync(request, request.MemberId, request.BookCopyId, request.Status, request.IsOverdue, ct));

    [HttpGet("loans/overdue")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdueLoans(CancellationToken ct)
        => Ok(await _circulation.GetOverdueLoansAsync(ct));

    [HttpGet("loans/{id:guid}")]
    public async Task<ActionResult<LoanDto>> GetLoanById(Guid id, CancellationToken ct)
    {
        var loan = await _circulation.GetLoanByIdAsync(id, ct);
        return loan == null ? NotFound() : Ok(loan);
    }

    [HttpPost("loans/issue")]
    public async Task<ActionResult<LoanDto>> IssueBook([FromBody] CreateLoanRequest request, CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        var loan = await _circulation.IssueBookAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetLoanById), new { id = loan.Id }, loan);
    }

    [HttpPost("loans/renew")]
    public async Task<ActionResult<LoanDto>> RenewLoan([FromBody] RenewLoanRequest request, CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        var loan = await _circulation.RenewLoanAsync(request, userId, ct);
        return loan == null ? NotFound() : Ok(loan);
    }

    [HttpPost("loans/return")]
    public async Task<ActionResult<LoanDto>> ReturnBook([FromBody] ReturnLoanRequest request, CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        var loan = await _circulation.ReturnBookAsync(request, userId, ct);
        return loan == null ? NotFound() : Ok(loan);
    }

    [HttpPost("loans/process-overdue")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ProcessOverdueLoans(CancellationToken ct)
    {
        await _circulation.ProcessOverdueLoansAsync(ct);
        return Ok(new { message = "Overdue loans processed" });
    }

    // ---------- Reservations ----------

    [HttpGet("reservations")]
    public async Task<ActionResult<PaginatedResult<ReservationDto>>> GetReservations([FromQuery] PaginationRequest request, [FromQuery] Guid? memberId, [FromQuery] Guid? bookId, [FromQuery] ReservationStatus? status, CancellationToken ct)
        => Ok(await _circulation.GetReservationsAsync(request, memberId, bookId, status, ct));

    [HttpGet("reservations/{id:guid}")]
    public async Task<ActionResult<ReservationDto>> GetReservationById(Guid id, CancellationToken ct)
    {
        var r = await _circulation.GetReservationByIdAsync(id, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPost("reservations")]
    public async Task<ActionResult<ReservationDto>> CreateReservation([FromBody] CreateReservationRequest request, CancellationToken ct)
    {
        var r = await _circulation.CreateReservationAsync(request, ct);
        return CreatedAtAction(nameof(GetReservationById), new { id = r.Id }, r);
    }

    [HttpPost("reservations/cancel")]
    public async Task<IActionResult> CancelReservation([FromBody] CancelReservationRequest request, CancellationToken ct)
        => await _circulation.CancelReservationAsync(request, ct) ? Ok() : NotFound();

    [HttpPost("reservations/process-expired")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ProcessExpiredReservations(CancellationToken ct)
    {
        await _circulation.ProcessExpiredReservationsAsync(ct);
        return Ok(new { message = "Expired reservations processed" });
    }

    // ---------- Fines ----------

    [HttpGet("fines")]
    public async Task<ActionResult<PaginatedResult<FineDto>>> GetFines([FromQuery] FinePaginationRequest request, CancellationToken ct)
        => Ok(await _circulation.GetFinesAsync(request, request.MemberId, request.Status, ct));

    [HttpGet("fines/{id:guid}")]
    public async Task<ActionResult<FineDto>> GetFineById(Guid id, CancellationToken ct)
    {
        var fine = await _circulation.GetFineByIdAsync(id, ct);
        return fine == null ? NotFound() : Ok(fine);
    }

    [HttpPost("fines")]
    public async Task<ActionResult<FineDto>> CreateFine([FromBody] CreateFineRequest request, CancellationToken ct)
    {
        var fine = await _circulation.CreateFineAsync(request, ct);
        return CreatedAtAction(nameof(GetFineById), new { id = fine.Id }, fine);
    }

    /// <summary>Waive or otherwise change fine status (Admin only).</summary>
    [HttpPatch("fines/{id:guid}/status")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<FineDto>> UpdateFineStatus(Guid id, [FromQuery] FineStatus status, CancellationToken ct)
    {
        var fine = await _circulation.UpdateFineStatusAsync(id, status, ct);
        return fine == null ? NotFound() : Ok(fine);
    }

    // ---------- Payments ----------

    [HttpGet("payments")]
    public async Task<ActionResult<PaginatedResult<PaymentDto>>> GetPayments([FromQuery] PaginationRequest request, [FromQuery] Guid? fineId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken ct)
        => Ok(await _circulation.GetPaymentsAsync(request, fineId, fromDate, toDate, ct));

    [HttpPost("payments")]
    public async Task<ActionResult<PaymentDto>> RecordPayment([FromBody] CreatePaymentRequest request, CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        var payment = await _circulation.RecordPaymentAsync(request, userId, ct);
        return Ok(payment);
    }
}
