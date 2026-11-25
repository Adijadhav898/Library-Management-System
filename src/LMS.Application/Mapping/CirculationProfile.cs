using AutoMapper;
using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Mapping;

public class CirculationProfile : Profile
{
    public CirculationProfile()
    {
        CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.BookCopy != null && src.BookCopy.Book != null ? src.BookCopy.Book.Title : string.Empty))
            .ForMember(dest => dest.CopyNumber, opt => opt.MapFrom(src => src.BookCopy != null ? src.BookCopy.CopyNumber : string.Empty))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null ? src.Member.FirstName + " " + src.Member.LastName : string.Empty))
            .ForMember(dest => dest.MemberNumber, opt => opt.MapFrom(src => src.Member != null ? src.Member.MemberNumber : string.Empty))
            .ForMember(dest => dest.IssuedByName, opt => opt.MapFrom(src => src.IssuedBy != null ? src.IssuedBy.FirstName + " " + src.IssuedBy.LastName : string.Empty))
            .ForMember(dest => dest.ReturnedByName, opt => opt.MapFrom(src => src.ReturnedBy != null ? src.ReturnedBy.FirstName + " " + src.ReturnedBy.LastName : string.Empty))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.Status == Domain.Enums.LoanStatus.Active && src.DueDate < DateTime.UtcNow));

        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null ? src.Member.FirstName + " " + src.Member.LastName : string.Empty))
            .ForMember(dest => dest.MemberNumber, opt => opt.MapFrom(src => src.Member != null ? src.Member.MemberNumber : string.Empty))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Fine, FineDto>()
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null ? src.Member.FirstName + " " + src.Member.LastName : string.Empty))
            .ForMember(dest => dest.MemberNumber, opt => opt.MapFrom(src => src.Member != null ? src.Member.MemberNumber : string.Empty))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Loan != null && src.Loan.BookCopy != null && src.Loan.BookCopy.Book != null ? src.Loan.BookCopy.Book.Title : string.Empty))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.Amount - src.AmountPaid));

        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.ModeName, opt => opt.MapFrom(src => src.Mode.ToString()));
    }
}