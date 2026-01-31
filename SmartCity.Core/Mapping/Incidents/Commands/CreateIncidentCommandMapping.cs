using SmartCity.AppCore.Features.Incidents.Commands.Models;
using SmartCity.AppCore.Features.Incidents.Commands.Results;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Mapping.Incidents
{
    public partial class IncidentProfile
    {
        public void CreateIncidentCommandMapping()
        {
            CreateMap<CreateIncidentCommand, Incident>();


            //CreateMap<Incident, CreateIncidentResponse>()
            //    .ForMember(dest => dest.Status,
            //          opt => opt.MapFrom(src => src.Status));


            // Entity → Response
            CreateMap<Incident, CreateIncidentResponse>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Assignment,
                    opt => opt.MapFrom(src =>
                        src.Assignments != null && src.Assignments.Any()
                            ? src.Assignments.OrderByDescending(a => a.AssignedAt).FirstOrDefault()
                            : null));

            // Assignment → AssignmentInfo
            CreateMap<Assignment, AssignmentInfo>()
                .ForMember(dest => dest.UnitName,
                    opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Name : string.Empty))
                .ForMember(dest => dest.UnitContact,
                    opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Contact : string.Empty));


            // New mapping for AI response
            CreateMap<Incident, CreateIncidentAIResponse>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Assignment,
                    opt => opt.MapFrom(src =>
                        src.Assignments != null && src.Assignments.Any()
                            ? src.Assignments.OrderByDescending(a => a.AssignedAt).FirstOrDefault()
                            : null));





        }
    }
}
