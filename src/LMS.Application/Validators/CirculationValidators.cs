using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.Application.Validators;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.BookCopyId)
            .NotEmpty().WithMessage("Book copy ID is required");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

public class ReturnLoanRequestValidator : AbstractValidator<ReturnLoanRequest>
{
    public ReturnLoanRequestValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required");

        RuleFor(x => x.ReturnedById)
            .NotEmpty().WithMessage("Returned by ID is required");

        RuleFor(x => x.ConditionNotes)
            .MaximumLength(1000).WithMessage("Condition notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ConditionNotes));
    }
}

public class RenewLoanRequestValidator : AbstractValidator<RenewLoanRequest>
{
    public RenewLoanRequestValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required");

        RuleFor(x => x.RenewedById)
            .NotEmpty().WithMessage("Renewed by ID is required");
    }
}

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required");
    }
}

public class UpdateReservationStatusRequestValidator : AbstractValidator<UpdateReservationStatusRequest>
{
    public UpdateReservationStatusRequestValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty().WithMessage("Reservation ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid reservation status");
    }
}

public class CreateFineRequestValidator : AbstractValidator<CreateFineRequest>
{
    public CreateFineRequestValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid fine type");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.IncurredDate)
            .NotEmpty().WithMessage("Incurred date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Incurred date cannot be in the future");
    }
}

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.FineId)
            .NotEmpty().WithMessage("Fine ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.PaymentMode)
            .IsInEnum().WithMessage("Invalid payment mode");

        RuleFor(x => x.TransactionId)
            .MaximumLength(200).WithMessage("Transaction reference cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.TransactionId));

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

public class CirculationPaginationRequestValidator : AbstractValidator<CirculationPaginationRequest>
{
    public CirculationPaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.MemberId)
            .NotNull().WithMessage("Member ID filter cannot be null")
            .When(x => x.MemberId.HasValue);

        RuleFor(x => x.BookCopyId)
            .NotNull().WithMessage("Book copy ID filter cannot be null")
            .When(x => x.BookCopyId.HasValue);

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status filter cannot be null")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.IsOverdue)
            .NotNull().WithMessage("IsOverdue filter cannot be null")
            .When(x => x.IsOverdue.HasValue);
    }
}

public class FinePaginationRequestValidator : AbstractValidator<FinePaginationRequest>
{
    public FinePaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.MemberId)
            .NotNull().WithMessage("Member ID filter cannot be null")
            .When(x => x.MemberId.HasValue);

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status filter cannot be null")
            .When(x => x.Status.HasValue);
    }
}