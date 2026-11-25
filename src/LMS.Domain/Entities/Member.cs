namespace LMS.Domain.Entities;

public class Member : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string MembershipNumber { get; set; } = string.Empty;
    public Guid MemberTypeId { get; set; }
    public MemberType MemberType { get; set; } = null!;
    public DateTime JoinDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Address { get; set; }
    public bool IsBlocked { get; set; } = false;
    public string? BlockReason { get; set; }

    // Navigation properties
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}