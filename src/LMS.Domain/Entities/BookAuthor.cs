namespace LMS.Domain.Entities;

public class BookAuthor : BaseEntity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}