using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.Application.Validators;

public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequest>
{
    public CreateMemberRequestValidator()
    {
        RuleFor(x => x.MemberNumber)
            .NotEmpty().WithMessage("Member number is required")
            .MaximumLength(50).WithMessage("Member number cannot exceed 50 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value")
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.MemberTypeId)
            .NotEmpty().WithMessage("Member type is required");

        RuleFor(x => x.MembershipStartDate)
            .NotEmpty().WithMessage("Membership start date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Membership start date cannot be in the future");

        RuleFor(x => x.MembershipEndDate)
            .GreaterThan(x => x.MembershipStartDate).WithMessage("Membership end date must be after start date")
            .When(x => x.MembershipEndDate.HasValue && x.MembershipStartDate.HasValue);
    }
}

public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequest>
{
    public UpdateMemberRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.MemberNumber)
            .NotEmpty().WithMessage("Member number is required")
            .MaximumLength(50).WithMessage("Member number cannot exceed 50 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value")
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.MemberTypeId)
            .NotEmpty().WithMessage("Member type is required");

        RuleFor(x => x.MembershipStartDate)
            .NotEmpty().WithMessage("Membership start date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Membership start date cannot be in the future");

        RuleFor(x => x.MembershipEndDate)
            .GreaterThan(x => x.MembershipStartDate).WithMessage("Membership end date must be after start date")
            .When(x => x.MembershipEndDate.HasValue && x.MembershipStartDate.HasValue);
    }
}

public class MemberPaginationRequestValidator : AbstractValidator<MemberPaginationRequest>
{
    public MemberPaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("IsActive filter cannot be null")
            .When(x => x.IsActive.HasValue);

        RuleFor(x => x.MemberTypeId)
            .NotNull().WithMessage("Member type ID filter cannot be null")
            .When(x => x.MemberTypeId.HasValue);
    }
}

public class RenewMembershipRequestValidator : AbstractValidator<RenewMembershipRequest>
{
    public RenewMembershipRequestValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.NewEndDate)
            .NotEmpty().WithMessage("New end date is required")
            .GreaterThan(DateTime.Today).WithMessage("New end date must be in the future");
    }
}

public class CreateMemberTypeRequestValidator : AbstractValidator<CreateMemberTypeRequest>
{
    public CreateMemberTypeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MaxBooksAllowed)
            .GreaterThan(0).WithMessage("Max books allowed must be greater than 0");

        RuleFor(x => x.LoanDurationDays)
            .GreaterThan(0).WithMessage("Loan duration must be greater than 0");

        RuleFor(x => x.MaxRenewalsAllowed)
            .GreaterThanOrEqualTo(0).WithMessage("Max renewals allowed cannot be negative");

        RuleFor(x => x.FineRatePerDay)
            .GreaterThanOrEqualTo(0).WithMessage("Fine rate per day cannot be negative");

        RuleFor(x => x.MembershipFee)
            .GreaterThanOrEqualTo(0).WithMessage("Membership fee cannot be negative");

        RuleFor(x => x.MembershipDurationDays)
            .GreaterThan(0).WithMessage("Membership duration must be greater than 0");
    }
}

public class UpdateMemberTypeRequestValidator : AbstractValidator<UpdateMemberTypeRequest>
{
    public UpdateMemberTypeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Member type ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MaxBooksAllowed)
            .GreaterThan(0).WithMessage("Max books allowed must be greater than 0");

        RuleFor(x => x.LoanDurationDays)
            .GreaterThan(0).WithMessage("Loan duration must be greater than 0");

        RuleFor(x => x.MaxRenewalsAllowed)
            .GreaterThanOrEqualTo(0).WithMessage("Max renewals allowed cannot be negative");

        RuleFor(x => x.FineRatePerDay)
            .GreaterThanOrEqualTo(0).WithMessage("Fine rate per day cannot be negative");

        RuleFor(x => x.MembershipFee)
            .GreaterThanOrEqualTo(0).WithMessage("Membership fee cannot be negative");

        RuleFor(x => x.MembershipDurationDays)
            .GreaterThan(0).WithMessage("Membership duration must be greater than 0");
    }
}