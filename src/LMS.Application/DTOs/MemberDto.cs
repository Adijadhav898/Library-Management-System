namespace LMS.Application.DTOs;

public class MemberDto : BaseDto
{
    public Guid UserId { get; set; }
    public string MembershipNumber { get; set; } = string.Empty;
    public string MemberNumber => MembershipNumber;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime MembershipDate { get; set; }
    public DateTime JoinDate => MembershipDate;
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public Guid MemberTypeId { get; set; }
    public string MemberTypeName { get; set; } = string.Empty;
    public int MaxBooksAllowed { get; set; }
    public int LoanDurationDays { get; set; }
    public decimal FinePerDay { get; set; }
    public int MaxRenewalsAllowed { get; set; }
    public int ReservationExpiryHours { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public int ActiveLoansCount { get; set; }
    public decimal TotalOutstandingFines { get; set; }
    public UserDto? User { get; set; }
    public decimal UnpaidFines => TotalOutstandingFines;
}

public class CreateMemberRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Guid MemberTypeId { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class UpdateMemberRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool? IsActive { get; set; }
    public Guid? MemberTypeId { get; set; }
}

public class MemberTypeDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxBooksAllowed { get; set; }
    public int LoanDurationDays { get; set; }
    public int MaxRenewalsAllowed { get; set; }
    public decimal FinePerDay { get; set; }
    public int ReservationExpiryHours { get; set; }
}

public class CreateMemberTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxBooksAllowed { get; set; }
    public int LoanDurationDays { get; set; }
    public int MaxRenewalsAllowed { get; set; }
    public decimal FinePerDay { get; set; }
    public int ReservationExpiryHours { get; set; }
}

public class UpdateMemberTypeRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? MaxBooksAllowed { get; set; }
    public int? LoanDurationDays { get; set; }
    public int? MaxRenewalsAllowed { get; set; }
    public decimal? FinePerDay { get; set; }
    public int? ReservationExpiryHours { get; set; }
}

public class MemberPaginationRequest : PaginationRequest
{
    public bool? IsActive { get; set; }
    public Guid? MemberTypeId { get; set; }
}

public class RenewMembershipRequest
{
    public Guid MemberId { get; set; }
    public DateTime NewEndDate { get; set; }
}