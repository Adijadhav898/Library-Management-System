using AutoMapper;
using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Mapping;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        // Report DTOs are primarily projections from repositories, not direct entity mappings
        // This profile can be extended if needed for any entity-to-report-DTO mappings
    }
}