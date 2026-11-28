namespace LMS.Domain.Entities;

using LMS.Domain.Enums;

public class Loan : BaseEntity
{
    public Guid BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public Guid IssuedById { get; set; }
    public User IssuedBy { get; set; } = null!;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int RenewalCount { get; set; } = 0;
    public int MaxRenewals { get; set; } = 3;
    public LoanStatus Status { get; set; } = LoanStatus.Active;
    public Guid? ReturnedByUserId { get; set; }
    public User? ReturnedByUser { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}