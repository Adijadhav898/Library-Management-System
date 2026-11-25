namespace LMS.Domain.Entities;

public class MemberType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int MaxBooksAllowed { get; set; }
    public int LoanDays { get; set; }
    public int MaxRenewals { get; set; }
    public decimal FinePerDay { get; set; }

    // Navigation properties
    public ICollection<Member> Members { get; set; } = new List<Member>();
}