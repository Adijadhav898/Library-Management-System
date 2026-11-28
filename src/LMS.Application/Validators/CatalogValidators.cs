using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ParentCategoryId)
            .NotEmpty().WithMessage("Parent category ID cannot be empty when specified")
            .When(x => x.ParentCategoryId.HasValue);
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ParentCategoryId)
            .NotEmpty().WithMessage("Parent category ID cannot be empty when specified")
            .When(x => x.ParentCategoryId.HasValue);
    }
}

public class CreatePublisherRequestValidator : AbstractValidator<CreatePublisherRequest>
{
    public CreatePublisherRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Website)
            .Matches(@"^https?:\/\/.+").WithMessage("Invalid website URL format")
            .When(x => !string.IsNullOrWhiteSpace(x.Website));
    }
}

public class UpdatePublisherRequestValidator : AbstractValidator<UpdatePublisherRequest>
{
    public UpdatePublisherRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Website)
            .Matches(@"^https?:\/\/.+").WithMessage("Invalid website URL format")
            .When(x => !string.IsNullOrWhiteSpace(x.Website));
    }
}

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Biography)
            .MaximumLength(2000).WithMessage("Biography cannot exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Biography));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.DateOfDeath)
            .GreaterThan(x => x.DateOfBirth).WithMessage("Date of death must be after date of birth")
            .When(x => x.DateOfDeath.HasValue && x.DateOfBirth.HasValue);
    }
}

public class UpdateAuthorRequestValidator : AbstractValidator<UpdateAuthorRequest>
{
    public UpdateAuthorRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Biography)
            .MaximumLength(2000).WithMessage("Biography cannot exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Biography));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.DateOfDeath)
            .GreaterThan(x => x.DateOfBirth).WithMessage("Date of death must be after date of birth")
            .When(x => x.DateOfDeath.HasValue && x.DateOfBirth.HasValue);
    }
}

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required")
            .Matches(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$").WithMessage("Invalid ISBN format")
            .When(x => !string.IsNullOrWhiteSpace(x.ISBN));

        RuleFor(x => x.Edition)
            .MaximumLength(50).WithMessage("Edition cannot exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Edition));

        RuleFor(x => x.PublicationYear)
            .GreaterThan(1000).WithMessage("Invalid publication year")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Publication year cannot be in the future")
            .When(x => x.PublicationYear.HasValue);

        RuleFor(x => x.Pages)
            .GreaterThan(0).WithMessage("Number of pages must be greater than 0")
            .When(x => x.Pages.HasValue);

        RuleFor(x => x.Language)
            .MaximumLength(50).WithMessage("Language cannot exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Language));

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.PublisherId)
            .NotEmpty().WithMessage("Publisher is required");

        RuleFor(x => x.AuthorIds)
            .NotNull().WithMessage("Authors cannot be null")
            .Must(x => x.Count > 0).WithMessage("At least one author is required");
    }
}

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required")
            .Matches(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$").WithMessage("Invalid ISBN format")
            .When(x => !string.IsNullOrWhiteSpace(x.ISBN));

        RuleFor(x => x.Edition)
            .MaximumLength(50).WithMessage("Edition cannot exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Edition));

        RuleFor(x => x.PublicationYear)
            .GreaterThan(1000).WithMessage("Invalid publication year")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Publication year cannot be in the future")
            .When(x => x.PublicationYear.HasValue);

        RuleFor(x => x.Pages)
            .GreaterThan(0).WithMessage("Number of pages must be greater than 0")
            .When(x => x.Pages.HasValue);

        RuleFor(x => x.Language)
            .MaximumLength(50).WithMessage("Language cannot exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Language));

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.PublisherId)
            .NotEmpty().WithMessage("Publisher is required");

        RuleFor(x => x.AuthorIds)
            .NotNull().WithMessage("Authors cannot be null")
            .Must(x => x.Count > 0).WithMessage("At least one author is required");
    }
}

public class CreateBookCopyRequestValidator : AbstractValidator<CreateBookCopyRequest>
{
    public CreateBookCopyRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required");

        RuleFor(x => x.CopyNumber)
            .NotEmpty().WithMessage("Copy number is required")
            .MaximumLength(50).WithMessage("Copy number cannot exceed 50 characters");

        RuleFor(x => x.AcquisitionDate)
            .NotEmpty().WithMessage("Acquisition date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Acquisition date cannot be in the future");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost cannot be negative")
            .When(x => x.Cost.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid copy status");
    }
}

public class UpdateBookCopyRequestValidator : AbstractValidator<UpdateBookCopyRequest>
{
    public UpdateBookCopyRequestValidator()
    {
        RuleFor(x => x.CopyNumber)
            .NotEmpty().WithMessage("Copy number is required")
            .MaximumLength(50).WithMessage("Copy number cannot exceed 50 characters");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost cannot be negative")
            .When(x => x.Cost.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid copy status");
    }
}

public class CatalogPaginationRequestValidator : AbstractValidator<CatalogPaginationRequest>
{
    public CatalogPaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.CategoryId)
            .NotNull().WithMessage("Category ID filter cannot be null")
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.PublisherId)
            .NotNull().WithMessage("Publisher ID filter cannot be null")
            .When(x => x.PublisherId.HasValue);
    }
}