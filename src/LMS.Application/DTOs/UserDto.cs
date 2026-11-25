namespace LMS.Application.DTOs;

public class UserDto : BaseDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
    public bool IsActive { get; set; } = true;
}

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? RoleIds { get; set; }
}

public class UserPaginationRequest : PaginationRequest
{
    public bool? IsActive { get; set; }
    public Guid? RoleId { get; set; }
}

public class AssignRolesRequest
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}