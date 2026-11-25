namespace LMS.Application.DTOs;

using LMS.Domain.Enums;

public class LoanDto : BaseDto
{
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;
    public Guid BookCopyId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public int RenewalCount { get; set; }
    public int MaxRenewals { get; set; }
    public Guid? IssuedByUserId { get; set; }
    public string IssuedByName { get; set; } = string.Empty;
    public Guid? ReturnedByUserId { get; set; }
    public string ReturnedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsOverdue => Status == LoanStatus.Active && DueDate < DateTime.UtcNow;
    public int DaysOverdue => IsOverdue ? (int)(DateTime.UtcNow - DueDate).TotalDays : 0;
}

public class CreateLoanRequest
{
    public Guid MemberId { get; set; }
    public Guid BookCopyId { get; set; }
    public string? Notes { get; set; }
}

public class RenewLoanRequest
{
    public Guid LoanId { get; set; }
}

public class ReturnLoanRequest
{
    public Guid LoanId { get; set; }
    public string? Notes { get; set; }
    public string Condition { get; set; } = "Good";
}

public class ReservationDto : BaseDto
{
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public Guid? BookCopyId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime? FulfilledAt { get; set; }
    public string? Notes { get; set; }
    public bool IsExpired => Status == ReservationStatus.Pending && ExpiryDate < DateTime.UtcNow;
}

public class CreateReservationRequest
{
    public Guid MemberId { get; set; }
    public Guid BookId { get; set; }
    public string? Notes { get; set; }
}

public class CancelReservationRequest
{
    public Guid ReservationId { get; set; }
}

public class UpdateReservationStatusRequest
{
    public Guid ReservationId { get; set; }
    public ReservationStatus Status { get; set; }
}

public class FineDto : BaseDto
{
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;
    public Guid? LoanId { get; set; }
    public FineType FineType { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public FineStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal OutstandingAmount => Amount - PaidAmount;
}

public class CreateFineRequest
{
    public Guid MemberId { get; set; }
    public Guid? LoanId { get; set; }
    public FineType FineType { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class PaymentDto : BaseDto
{
    public Guid FineId { get; set; }
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public Guid ReceivedByUserId { get; set; }
    public string ReceivedByName { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreatePaymentRequest
{
    public Guid FineId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class CirculationPaginationRequest : PaginationRequest
{
    public Guid? MemberId { get; set; }
    public Guid? BookCopyId { get; set; }
    public LoanStatus? Status { get; set; }
    public bool? IsOverdue { get; set; }
}

public class FinePaginationRequest : PaginationRequest
{
    public Guid? MemberId { get; set; }
    public FineStatus? Status { get; set; }
}