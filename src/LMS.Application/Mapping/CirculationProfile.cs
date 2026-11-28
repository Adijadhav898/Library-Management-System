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
            .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.BookCopy != null ? src.BookCopy.Barcode : string.Empty))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null && src.Member.User != null ? src.Member.User.FirstName + " " + src.Member.User.LastName : string.Empty))
            .ForMember(dest => dest.MembershipNumber, opt => opt.MapFrom(src => src.Member != null ? src.Member.MembershipNumber : string.Empty))
            .ForMember(dest => dest.IssuedByUserId, opt => opt.MapFrom(src => src.IssuedById))
            .ForMember(dest => dest.IssuedByName, opt => opt.MapFrom(src => src.IssuedBy != null ? src.IssuedBy.FirstName + " " + src.IssuedBy.LastName : string.Empty))
            .ForMember(dest => dest.ReturnedByName, opt => opt.MapFrom(src => src.ReturnedByUser != null ? src.ReturnedByUser.FirstName + " " + src.ReturnedByUser.LastName : string.Empty));

        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null && src.Member.User != null ? src.Member.User.FirstName + " " + src.Member.User.LastName : string.Empty))
            .ForMember(dest => dest.MembershipNumber, opt => opt.MapFrom(src => src.Member != null ? src.Member.MembershipNumber : string.Empty))
            .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.BookCopy != null ? src.BookCopy.Barcode : string.Empty))
            .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.ReservedAt))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiresAt));

        CreateMap<Fine, FineDto>()
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null && src.Member.User != null ? src.Member.User.FirstName + " " + src.Member.User.LastName : string.Empty))
            .ForMember(dest => dest.MembershipNumber, opt => opt.MapFrom(src => src.Member != null ? src.Member.MembershipNumber : string.Empty))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));

        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.Fine != null ? src.Fine.MemberId : Guid.Empty))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Fine != null && src.Fine.Member != null && src.Fine.Member.User != null ? src.Fine.Member.User.FirstName + " " + src.Fine.Member.User.LastName : string.Empty))
            .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionRef ?? string.Empty))
            .ForMember(dest => dest.ReceivedByUserId, opt => opt.MapFrom(src => src.ReceivedById))
            .ForMember(dest => dest.ReceivedByName, opt => opt.MapFrom(src => src.ReceivedBy != null ? src.ReceivedBy.FirstName + " " + src.ReceivedBy.LastName : string.Empty))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaidAt))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes ?? string.Empty));
    }
}
