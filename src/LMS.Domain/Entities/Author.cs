namespace LMS.Domain.Entities;

public class Author : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public string? Nationality { get; set; }

    // Navigation properties
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    // Computed property
    public string FullName => $"{FirstName} {LastName}".Trim();
}