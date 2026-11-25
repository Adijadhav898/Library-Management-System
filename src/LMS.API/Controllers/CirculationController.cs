using LMS.Application.DTOs.Circulation;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "LibrarianOrAdmin")]
[Produces("application/json")]
public class CirculationController : ControllerBase
{
    private readonly ICirculationService _circulationService;
    private readonly ILogger<CirculationController> _logger;

    public CirculationController(ICirculationService circulationService, ILogger<CirculationController> logger)
    {
        _circulationService = circulationService;
        _logger = logger;
    }

    #region Loans

    /// <summary>
    /// Get all loans with pagination and filters
    /// </summary>
    [HttpGet("loans")]
    [ProducesResponseType(typeof(PagedResultDto<LoanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<LoanDto>>> GetLoans(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? memberId = null,
        [FromQuery] Guid? bookCopyId = null,
        [FromQuery] string? status = null,
        [FromQuery] bool? isOverdue = null)
    {
        var result = await _circulationService.GetLoansAsync(page, pageSize, search, memberId, bookCopyId, status, isOverdue);
        return Ok(result);
    }

    /// <summary>
    /// Get loan by ID
    /// </summary>
    [HttpGet("loans/{id:guid}")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> GetLoan(Guid id)
    {
        var loan = await _circulationService.GetLoanByIdAsync(id);
        if (loan == null)
        {
            return NotFound(new { message = "Loan not found" });
        }
        return Ok(loan);
    }

    /// <summary>
    /// Issue a new loan
    /// </summary>
    [HttpPost("loans")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanDto>> IssueLoan([FromBody] IssueLoanRequestDto request)
    {
        try
        {
            var loan = await _circulationService.IssueLoanAsync(request);
            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Return a loan
    /// </summary>
    [HttpPost("loans/{id:guid}/return")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> ReturnLoan(Guid id, [FromBody] ReturnLoanRequestDto request)
    {
        try
        {
            var loan = await _circulationService.ReturnLoanAsync(id, request);
            if (loan == null)
            {
                return NotFound(new { message = "Loan not found" });
            }
            return Ok(loan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Renew a loan
    /// </summary>
    [HttpPost("loans/{id:guid}/renew")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> RenewLoan(Guid id, [FromBody] RenewLoanRequestDto request)
    {
        try
        {
            var loan = await _circulationService.RenewLoanAsync(id, request);
            if (loan == null)
            {
                return NotFound(new { message = "Loan not found" });
            }
            return Ok(loan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get overdue loans
    /// </summary>
    [HttpGet("loans/overdue")]
    [ProducesResponseType(typeof(IEnumerable<LoanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdueLoans()
    {
        var loans = await _circulationService.GetOverdueLoansAsync();
        return Ok(loans);
    }

    /// <summary>
    /// Process overdue fines (background job trigger)
    /// </summary>
    [HttpPost("loans/process-overdue-fines")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessOverdueFines()
    {
        await _circulationService.ProcessOverdueFinesAsync();
        return Ok(new { message = "Overdue fines processed successfully" });
    }

    /// <summary>
    /// Process expired reservations (background job trigger)
    /// </summary>
    [HttpPost("loans/process-expired-reservations")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessExpiredReservations()
    {
        await _circulationService.ProcessExpiredReservationsAsync();
        return Ok(new { message = "Expired reservations processed successfully" });
    }

    #endregion

    #region Reservations

    /// <summary>
    /// Get all reservations with pagination and filters
    /// </summary>
    [HttpGet("reservations")]
    [ProducesResponseType(typeof(PagedResultDto<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ReservationDto>>> GetReservations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? memberId = null,
        [FromQuery] Guid? bookId = null,
        [FromQuery] string? status = null)
    {
        var result = await _circulationService.GetReservationsAsync(page, pageSize, search, memberId, bookId, status);
        return Ok(result);
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("reservations/{id:guid}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> GetReservation(Guid id)
    {
        var reservation = await _circulationService.GetReservationByIdAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Reservation not found" });
        }
        return Ok(reservation);
    }

    /// <summary>
    /// Create a new reservation
    /// </summary>
    [HttpPost("reservations")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReservationDto>> CreateReservation([FromBody] CreateReservationRequestDto request)
    {
        try
        {
            var reservation = await _circulationService.CreateReservationAsync(request);
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpPost("reservations/{id:guid}/cancel")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> CancelReservation(Guid id)
    {
        var reservation = await _circulationService.CancelReservationAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Reservation not found" });
        }
        return Ok(reservation);
    }

    /// <summary>
    /// Fulfill a reservation (when book becomes available)
    /// </summary>
    [HttpPost("reservations/{id:guid}/fulfill")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> FulfillReservation(Guid id)
    {
        var reservation = await _circulationService.FulfillReservationAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Reservation not found" });
        }
        return Ok(reservation);
    }

    #endregion

    #region Fines

    /// <summary>
    /// Get all fines with pagination and filters
    /// </summary>
    [HttpGet("fines")]
    [ProducesResponseType(typeof(PagedResultDto<FineDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<FineDto>>> GetFines(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? memberId = null,
        [FromQuery] string? status = null)
    {
        var result = await _circulationService.GetFinesAsync(page, pageSize, memberId, status);
        return Ok(result);
    }

    /// <summary>
    /// Get fine by ID
    /// </summary>
    [HttpGet("fines/{id:guid}")]
    [ProducesResponseType(typeof(FineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FineDto>> GetFine(Guid id)
    {
        var fine = await _circulationService.GetFineByIdAsync(id);
        if (fine == null)
        {
            return NotFound(new { message = "Fine not found" });
        }
        return Ok(fine);
    }

    /// <summary>
    /// Pay a fine
    /// </summary>
    [HttpPost("fines/{id:guid}/pay")]
    [ProducesResponseType(typeof(FineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FineDto>> PayFine(Guid id, [FromBody] PayFineRequestDto request)
    {
        try
        {
            var fine = await _circulationService.PayFineAsync(id, request);
            if (fine == null)
            {
                return NotFound(new { message = "Fine not found" });
            }
            return Ok(fine);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Waive a fine (Admin only)
    /// </summary>
    [HttpPost("fines/{id:guid}/waive")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(FineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FineDto>> WaiveFine(Guid id, [FromBody] WaiveFineRequestDto request)
    {
        try
        {
            var fine = await _circulationService.WaiveFineAsync(id, request);
            if (fine == null)
            {
                return NotFound(new { message = "Fine not found" });
            }
            return Ok(fine);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get member fines summary
    /// </summary>
    [HttpGet("fines/member/{memberId:guid}/summary")]
    [ProducesResponseType(typeof(MemberFinesSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberFinesSummaryDto>> GetMemberFinesSummary(Guid memberId)
    {
        var summary = await _circulationService.GetMemberFinesSummaryAsync(memberId);
        if (summary == null)
        {
            return NotFound(new { message = "Member not found" });
        }
        return Ok(summary);
    }

    #endregion
}