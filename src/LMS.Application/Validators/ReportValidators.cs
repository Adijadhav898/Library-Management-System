using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.Application.Validators;

public class ReportFilterRequestValidator : AbstractValidator<ReportFilterRequest>
{
    public ReportFilterRequestValidator()
    {
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate).WithMessage("From date must be before or equal to To date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("To date must be after or equal to From date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.MemberId)
            .NotNull().WithMessage("Member ID filter cannot be null")
            .When(x => x.MemberId.HasValue);

        RuleFor(x => x.CategoryId)
            .NotNull().WithMessage("Category ID filter cannot be null")
            .When(x => x.CategoryId.HasValue);
    }
}

public class MonthlyStatsRequestValidator : AbstractValidator<MonthlyStatsRequest>
{
    public MonthlyStatsRequestValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThan(2000).WithMessage("Year must be greater than 2000")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Year cannot be more than next year");
    }
}

public class TopBorrowedBooksRequestValidator : AbstractValidator<TopBorrowedBooksRequest>
{
    public TopBorrowedBooksRequestValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0).WithMessage("Count must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Count cannot exceed 100");
    }
}