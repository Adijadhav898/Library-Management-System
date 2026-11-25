namespace LMS.Domain.Entities;

using LMS.Domain.Enums;

public class BookCopy : BaseEntity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public string Barcode { get; set; } = string.Empty;
    public string CopyNumber { get; set; } = string.Empty;
    public CopyStatus Status { get; set; } = CopyStatus.Available;
    public string? Location { get; set; }
    public string? Condition { get; set; }
    public DateTime AcquisitionDate { get; set; } = DateTime.UtcNow;
    public decimal Cost { get; set; }

    // Navigation properties
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}