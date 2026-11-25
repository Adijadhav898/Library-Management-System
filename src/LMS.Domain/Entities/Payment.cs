namespace LMS.Domain.Entities;

using LMS.Domain.Enums;

public class Payment : BaseEntity
{
    public Guid FineId { get; set; }
    public Fine Fine { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string? TransactionRef { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public Guid ReceivedById { get; set; }
    public User ReceivedBy { get; set; } = null!;
}