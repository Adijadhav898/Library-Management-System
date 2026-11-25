using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.Application.Validators;

public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid notification type");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority");

        RuleFor(x => x.RelatedEntityId)
            .NotEmpty().WithMessage("Related entity ID cannot be empty when specified")
            .When(x => x.RelatedEntityId.HasValue);
    }
}

public class NotificationPaginationRequestValidator : AbstractValidator<NotificationPaginationRequest>
{
    public NotificationPaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.IsRead)
            .NotNull().WithMessage("IsRead filter cannot be null")
            .When(x => x.IsRead.HasValue);

        RuleFor(x => x.Type)
            .NotNull().WithMessage("Type filter cannot be null")
            .When(x => x.Type.HasValue);
    }
}