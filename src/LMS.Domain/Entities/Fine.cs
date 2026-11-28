namespace LMS.Domain.Entities;

using LMS.Domain.Enums;

public class Fine : BaseEntity
{
    public Guid LoanId { get; set; }
    public Loan Loan { get; set; } = null!;
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public decimal Amount { get; set; }
    public FineType FineType { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Description { get; set; }
    public FineStatus Status { get; set; } = FineStatus.Unpaid;

    // Navigation properties
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}