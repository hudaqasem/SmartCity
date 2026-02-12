using AutoMapper;
using SmartCity.AppCore.Features.Admin.Queries.Results;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Mapping.Admin
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            // Admin Monitoring mappings
            CreateMap<ResponseUnit, UnitAvailabilityResponse>();

            CreateMap<Incident, WaitingIncidentResponse>()
                .ForMember(dest => dest.CreatedAt,
                          opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ReportedBy,
                          opt => opt.MapFrom(src => src.ReportedByUser != null
                              ? $"{src.ReportedByUser.FirstName} {src.ReportedByUser.LastName}"
                              : "Unknown"))
                .ForMember(dest => dest.Location,
                          opt => opt.MapFrom(src => $"{src.Latitude}, {src.Longitude}"));
        }

    }
}