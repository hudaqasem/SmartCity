using SmartCity.AppCore.Features.Incidents.Queries.Results;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Mapping.Incidents
{
    public partial class IncidentProfile
    {
        public void GetIncidentMapping()
        {
            CreateMap<Incident, GetIncidentResponse>()
                .ForMember(dest => dest.Status,
                      opt => opt.MapFrom(src => src.Status));
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
