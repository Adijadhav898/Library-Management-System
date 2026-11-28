namespace LMS.Domain.Entities;

using LMS.Domain.Enums;

public class Reservation : BaseEntity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    public DateTime? NotifiedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }
    public DateTime? FulfilledAt { get; set; }
    
    public Guid? BookCopyId { get; set; }
    public BookCopy? BookCopy { get; set; }
}