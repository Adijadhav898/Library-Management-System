using AutoMapper;
using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Mapping;

public class MemberProfile : Profile
{
    public MemberProfile()
    {
        CreateMap<Member, MemberDto>()
            .ForMember(dest => dest.MemberNumber, opt => opt.MapFrom(src => src.MembershipNumber))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User != null ? src.User.FirstName : string.Empty))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User != null ? src.User.LastName : string.Empty))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : src.Address))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User != null ? src.User.Address : src.Address))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User != null ? src.User.DateOfBirth ?? DateTime.MinValue : DateTime.MinValue))
            .ForMember(dest => dest.MembershipDate, opt => opt.MapFrom(src => src.JoinDate))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsBlocked && src.User != null && src.User.IsActive))
            .ForMember(dest => dest.MemberTypeName, opt => opt.MapFrom(src => src.MemberType != null ? src.MemberType.Name : string.Empty))
            .ForMember(dest => dest.MaxBooksAllowed, opt => opt.MapFrom(src => src.MemberType != null ? src.MemberType.MaxBooksAllowed : 0))
            .ForMember(dest => dest.LoanDurationDays, opt => opt.MapFrom(src => src.MemberType != null ? src.MemberType.LoanDays : 0))
            .ForMember(dest => dest.FinePerDay, opt => opt.MapFrom(src => src.MemberType != null ? src.MemberType.FinePerDay : 0))
            .ForMember(dest => dest.MaxRenewalsAllowed, opt => opt.MapFrom(src => src.MemberType != null ? src.MemberType.MaxRenewals : 0))
            .ForMember(dest => dest.ReservationExpiryHours, opt => opt.MapFrom(src => 48)) // Default value since MemberType doesn't have this
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            .ForMember(dest => dest.ActiveLoansCount, opt => opt.MapFrom(src => src.Loans != null ? src.Loans.Count(l => l.Status == LMS.Domain.Enums.LoanStatus.Active) : 0))
            .ForMember(dest => dest.TotalOutstandingFines, opt => opt.MapFrom(src => src.Fines != null ? src.Fines.Where(f => f.Status == LMS.Domain.Enums.FineStatus.Unpaid || f.Status == LMS.Domain.Enums.FineStatus.PartiallyPaid).Sum(f => f.Amount - f.PaidAmount) : 0));
        CreateMap<CreateMemberRequest, Member>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.MembershipNumber, opt => opt.Ignore())
            .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate ?? DateTime.UtcNow.AddYears(1)))
            .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address ?? string.Empty));

        CreateMap<UpdateMemberRequest, Member>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<MemberType, MemberTypeDto>()
            .ForMember(dest => dest.LoanDurationDays, opt => opt.MapFrom(src => src.LoanDays))
            .ForMember(dest => dest.MaxRenewalsAllowed, opt => opt.MapFrom(src => src.MaxRenewals))
            .ForMember(dest => dest.ReservationExpiryHours, opt => opt.MapFrom(src => 48));

        CreateMap<CreateMemberTypeRequest, MemberType>()
            .ForMember(dest => dest.LoanDays, opt => opt.MapFrom(src => src.LoanDurationDays))
            .ForMember(dest => dest.MaxRenewals, opt => opt.MapFrom(src => src.MaxRenewalsAllowed));

        CreateMap<UpdateMemberTypeRequest, MemberType>()
            .ForMember(dest => dest.LoanDays, opt => opt.MapFrom(src => src.LoanDurationDays))
            .ForMember(dest => dest.MaxRenewals, opt => opt.MapFrom(src => src.MaxRenewalsAllowed))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}