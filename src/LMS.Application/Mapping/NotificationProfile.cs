using AutoMapper;
using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Mapping;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.RelatedEntityName, opt => opt.MapFrom(src => src.RelatedEntityType));
    }
}