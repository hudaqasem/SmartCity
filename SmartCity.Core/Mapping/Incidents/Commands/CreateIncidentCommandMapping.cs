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
                           : null))
               .ForMember(dest => dest.AIDetection, opt => opt.Ignore());  // Set manually




            CreateMap<CreateIncidentWithAICommand, Incident>()
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
               .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
               .ForMember(dest => dest.ReportedByUserId, opt => opt.MapFrom(src => src.ReportedByUserId))
               .ForMember(dest => dest.Status, opt => opt.Ignore())  // Set by service
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())  // Auto-set
               .ForMember(dest => dest.Id, opt => opt.Ignore());  // Auto-generated




        }
    }
}
