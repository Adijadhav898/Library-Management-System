namespace LMS.Application.DTOs;

using LMS.Domain.Enums;

public class CategoryDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int BooksCount { get; set; }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateCategoryRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class PublisherDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
    public int BooksCount { get; set; }
}

public class CreatePublisherRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}

public class UpdatePublisherRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
}

public class AuthorDto : BaseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public string? Nationality { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public int BooksCount { get; set; }
}

public class CreateAuthorRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public string? Nationality { get; set; }
}

public class UpdateAuthorRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public string? Nationality { get; set; }
}

public class BookDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PublicationYear { get; set; }
    public string? Language { get; set; }
    public int PageCount { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid PublisherId { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    public List<AuthorDto> Authors { get; set; } = new();
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}

public class BookSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<string> AuthorNames { get; set; } = new();
    public int AvailableCopies { get; set; }
}

public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PublicationYear { get; set; }
    public string? Language { get; set; }
    public int PageCount { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PublisherId { get; set; }
    public List<Guid> AuthorIds { get; set; } = new();
}

public class UpdateBookRequest
{
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public int? PublicationYear { get; set; }
    public string? Language { get; set; }
    public int? PageCount { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? PublisherId { get; set; }
    public List<Guid>? AuthorIds { get; set; }
}

public class BookCopyDto : BaseDto
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string CopyNumber { get; set; } = string.Empty;
    public CopyStatus Status { get; set; }
    public string? Location { get; set; }
    public string? Barcode { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public decimal? Price { get; set; }
    public string? Notes { get; set; }
}

public class CreateBookCopyRequest
{
    public Guid BookId { get; set; }
    public string CopyNumber { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Barcode { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public decimal? Price { get; set; }
    public string? Notes { get; set; }
}

public class UpdateBookCopyRequest
{
    public CopyStatus? Status { get; set; }
    public string? Location { get; set; }
    public string? Barcode { get; set; }
    public string? Notes { get; set; }
}

public class CatalogPaginationRequest : PaginationRequest
{
    public Guid? CategoryId { get; set; }
    public Guid? PublisherId { get; set; }
}