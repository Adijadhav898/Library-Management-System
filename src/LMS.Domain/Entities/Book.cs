namespace LMS.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;
    public string? Language { get; set; }
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public int? Pages { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? ShelfLocation { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }

    // Navigation properties
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}