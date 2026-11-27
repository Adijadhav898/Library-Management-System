using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberTypeRepository _memberTypeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IFineRepository _fineRepository;

    public MemberService(
        IMemberRepository memberRepository,
        IMemberTypeRepository memberTypeRepository,
        IUserRepository userRepository,
        ILoanRepository loanRepository,
        IFineRepository fineRepository)
    {
        _memberRepository = memberRepository;
        _memberTypeRepository = memberTypeRepository;
        _userRepository = userRepository;
        _loanRepository = loanRepository;
        _fineRepository = fineRepository;
    }

    public async Task<PaginatedResult<MemberDto>> GetAllAsync(PaginationRequest request, bool? isActive = null, Guid? memberTypeId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var (members, totalCount) = await _memberRepository.GetPagedAsync(request, isActive, memberTypeId, search, cancellationToken);

        var memberDtos = new List<MemberDto>();
        foreach (var member in members)
        {
            var activeLoansCount = await _loanRepository.GetActiveLoansCountByMemberAsync(member.Id, cancellationToken);
            var unpaidFines = await _fineRepository.GetTotalOutstandingByMemberAsync(member.Id, cancellationToken);

            memberDtos.Add(new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                MembershipNumber = member.MembershipNumber,
                MemberTypeId = member.MemberTypeId,
                MemberTypeName = member.MemberType?.Name ?? string.Empty,
                JoinDate = member.JoinDate,
                ExpiryDate = member.ExpiryDate,
                IsActive = !member.IsBlocked,
                User = member.User != null ? new UserDto
                {
                    Id = member.User.Id,
                    Email = member.User.Email,
                    FirstName = member.User.FirstName,
                    LastName = member.User.LastName,
                    PhoneNumber = member.User.PhoneNumber,
                    Address = member.User.Address,
                    DateOfBirth = member.User.DateOfBirth,
                    ProfileImageUrl = member.User.ProfileImageUrl,
                    IsActive = member.User.IsActive,
                    EmailConfirmed = member.User.EmailConfirmed,
                    Roles = new List<string>(),
                    CreatedAt = member.User.CreatedAt
                } : null,
                ActiveLoansCount = activeLoansCount,
                UnpaidFines = unpaidFines
            });
        }

        return new PaginatedResult<MemberDto>
        {
            Items = memberDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<MemberDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByIdAsync(id, cancellationToken);
        if (member == null)
            return null;

        var activeLoansCount = await _loanRepository.GetActiveLoansCountByMemberAsync(member.Id, cancellationToken);
        var unpaidFines = await _fineRepository.GetTotalOutstandingByMemberAsync(member.Id, cancellationToken);

        return new MemberDto
        {
            Id = member.Id,
            UserId = member.UserId,
            MembershipNumber = member.MembershipNumber,
            MemberTypeId = member.MemberTypeId,
            MemberTypeName = member.MemberType?.Name ?? string.Empty,
            JoinDate = member.JoinDate,
            ExpiryDate = member.ExpiryDate,
            IsActive = !member.IsBlocked,
            User = member.User != null ? new UserDto
            {
                Id = member.User.Id,
                Email = member.User.Email,
                FirstName = member.User.FirstName,
                LastName = member.User.LastName,
                PhoneNumber = member.User.PhoneNumber,
                Address = member.User.Address,
                DateOfBirth = member.User.DateOfBirth,
                ProfileImageUrl = member.User.ProfileImageUrl,
                IsActive = member.User.IsActive,
                EmailConfirmed = member.User.EmailConfirmed,
                Roles = new List<string>(),
                CreatedAt = member.User.CreatedAt
            } : null,
            ActiveLoansCount = activeLoansCount,
            UnpaidFines = unpaidFines
        };
    }

    public async Task<MemberDto?> GetByMemberNumberAsync(string memberNumber, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByMemberNumberAsync(memberNumber, cancellationToken);
        if (member == null)
            return null;

        var activeLoansCount = await _loanRepository.GetActiveLoansCountByMemberAsync(member.Id, cancellationToken);
        var unpaidFines = await _fineRepository.GetTotalOutstandingByMemberAsync(member.Id, cancellationToken);

        return new MemberDto
        {
            Id = member.Id,
            UserId = member.UserId,
            MembershipNumber = member.MembershipNumber,
            MemberTypeId = member.MemberTypeId,
            MemberTypeName = member.MemberType?.Name ?? string.Empty,
            JoinDate = member.JoinDate,
            ExpiryDate = member.ExpiryDate,
            IsActive = !member.IsBlocked,
            User = member.User != null ? new UserDto
            {
                Id = member.User.Id,
                Email = member.User.Email,
                FirstName = member.User.FirstName,
                LastName = member.User.LastName,
                PhoneNumber = member.User.PhoneNumber,
                Address = member.User.Address,
                DateOfBirth = member.User.DateOfBirth,
                ProfileImageUrl = member.User.ProfileImageUrl,
                IsActive = member.User.IsActive,
                EmailConfirmed = member.User.EmailConfirmed,
                Roles = new List<string>(),
                CreatedAt = member.User.CreatedAt
            } : null,
            ActiveLoansCount = activeLoansCount,
            UnpaidFines = unpaidFines
        };
    }

    public async Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var memberType = await _memberTypeRepository.GetByIdAsync(request.MemberTypeId, cancellationToken);
        if (memberType == null)
            throw new InvalidOperationException("Member type not found");

        var existingMember = await _memberRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existingMember != null)
            throw new InvalidOperationException("User already has a membership");

        var membershipNumber = $"MEM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var isUnique = await _memberRepository.IsMemberNumberUniqueAsync(membershipNumber, null, cancellationToken);
        if (!isUnique)
            throw new InvalidOperationException("Membership number already exists");

        var member = new Member
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            MembershipNumber = membershipNumber,
            MemberTypeId = request.MemberTypeId,
            JoinDate = DateTime.UtcNow,
            ExpiryDate = request.ExpiryDate ?? DateTime.UtcNow.AddYears(1),
            Address = request.Address,
            CreatedAt = DateTime.UtcNow
        };

        await _memberRepository.AddAsync(member, cancellationToken);

        return await GetByIdAsync(member.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create member");
    }

    public async Task<MemberDto?> UpdateAsync(Guid id, UpdateMemberRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByIdAsync(id, cancellationToken);
        if (member == null)
            return null;

        if (request.MemberTypeId.HasValue)
        {
            var memberType = await _memberTypeRepository.GetByIdAsync(request.MemberTypeId.Value, cancellationToken);
            if (memberType == null)
                throw new InvalidOperationException("Member type not found");
            member.MemberTypeId = request.MemberTypeId.Value;
        }

        if (request.ExpiryDate.HasValue)
            member.ExpiryDate = request.ExpiryDate.Value;

        if (request.IsActive.HasValue)
            member.IsBlocked = !request.IsActive.Value;

        member.UpdatedAt = DateTime.UtcNow;
        await _memberRepository.UpdateAsync(member, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByIdAsync(id, cancellationToken);
        if (member == null)
            return false;

        member.IsDeleted = true;
        member.UpdatedAt = DateTime.UtcNow;
        await _memberRepository.UpdateAsync(member, cancellationToken);
        return true;
    }

    public async Task<bool> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByIdAsync(id, cancellationToken);
        if (member == null)
            return false;

        member.IsBlocked = !member.IsBlocked;
        member.UpdatedAt = DateTime.UtcNow;
        await _memberRepository.UpdateAsync(member, cancellationToken);
        return true;
    }

    public async Task<PaginatedResult<MemberTypeDto>> GetMemberTypesAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var (memberTypes, totalCount) = await _memberTypeRepository.GetPagedAsync(request, cancellationToken);

        var memberTypeDtos = memberTypes.Select(mt => new MemberTypeDto
        {
            Id = mt.Id,
            Name = mt.Name,
            MaxBooksAllowed = mt.MaxBooksAllowed,
            LoanDurationDays = mt.LoanDays,
            MaxRenewalsAllowed = mt.MaxRenewals,
            ReservationExpiryHours = mt.ReservationExpiryHours,
            FinePerDay = mt.FinePerDay,
            CreatedAt = mt.CreatedAt
        }).ToList();

        return new PaginatedResult<MemberTypeDto>
        {
            Items = memberTypeDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<MemberTypeDto?> GetMemberTypeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var memberType = await _memberTypeRepository.GetByIdAsync(id, cancellationToken);
        if (memberType == null)
            return null;

        return new MemberTypeDto
        {
            Id = memberType.Id,
            Name = memberType.Name,
            MaxBooksAllowed = memberType.MaxBooksAllowed,
            LoanDurationDays = memberType.LoanDays,
            MaxRenewalsAllowed = memberType.MaxRenewals,
            ReservationExpiryHours = memberType.ReservationExpiryHours,
            FinePerDay = memberType.FinePerDay,
            CreatedAt = memberType.CreatedAt
        };
    }

    public async Task<MemberTypeDto> CreateMemberTypeAsync(CreateMemberTypeRequest request, CancellationToken cancellationToken = default)
    {
        var memberType = new MemberType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            MaxBooksAllowed = request.MaxBooksAllowed,
            LoanDays = request.LoanDurationDays,
            MaxRenewals = request.MaxRenewalsAllowed,
            FinePerDay = request.FinePerDay,
            ReservationExpiryHours = request.ReservationExpiryHours,
            CreatedAt = DateTime.UtcNow
        };

        await _memberTypeRepository.AddAsync(memberType, cancellationToken);

        return new MemberTypeDto
        {
            Id = memberType.Id,
            Name = memberType.Name,
            MaxBooksAllowed = memberType.MaxBooksAllowed,
            LoanDurationDays = memberType.LoanDays,
            MaxRenewalsAllowed = memberType.MaxRenewals,
            ReservationExpiryHours = memberType.ReservationExpiryHours,
            FinePerDay = memberType.FinePerDay,
            CreatedAt = memberType.CreatedAt
        };
    }

    public async Task<MemberTypeDto?> UpdateMemberTypeAsync(Guid id, UpdateMemberTypeRequest request, CancellationToken cancellationToken = default)
    {
        var memberType = await _memberTypeRepository.GetByIdAsync(id, cancellationToken);
        if (memberType == null)
            return null;

        memberType.Name = request.Name ?? memberType.Name;
        memberType.MaxBooksAllowed = request.MaxBooksAllowed ?? memberType.MaxBooksAllowed;
        memberType.LoanDays = request.LoanDurationDays ?? memberType.LoanDays;
        memberType.MaxRenewals = request.MaxRenewalsAllowed ?? memberType.MaxRenewals;
        memberType.FinePerDay = request.FinePerDay ?? memberType.FinePerDay;
        memberType.ReservationExpiryHours = request.ReservationExpiryHours ?? memberType.ReservationExpiryHours;
        memberType.UpdatedAt = DateTime.UtcNow;

        await _memberTypeRepository.UpdateAsync(memberType, cancellationToken);

        return new MemberTypeDto
        {
            Id = memberType.Id,
            Name = memberType.Name,
            MaxBooksAllowed = memberType.MaxBooksAllowed,
            LoanDurationDays = memberType.LoanDays,
            MaxRenewalsAllowed = memberType.MaxRenewals,
            ReservationExpiryHours = memberType.ReservationExpiryHours,
            FinePerDay = memberType.FinePerDay,
            CreatedAt = memberType.CreatedAt
        };
    }

    public async Task<bool> DeleteMemberTypeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var memberType = await _memberTypeRepository.GetByIdAsync(id, cancellationToken);
        if (memberType == null)
            return false;

        memberType.IsDeleted = true;
        memberType.UpdatedAt = DateTime.UtcNow;
        await _memberTypeRepository.UpdateAsync(memberType, cancellationToken);
        return true;
    }
}