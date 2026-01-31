using SmartCity.AppCore.Features.Incidents.Queries.Results;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Mapping.Incidents
{
    public partial class IncidentProfile
    {
        public void GetMyIncidentMapping()
        {
            CreateMap<Incident, GetIncidentDetailsResponse>()
                  .ForMember(dest => dest.Status,
                      opt => opt.MapFrom(src => src.Status));
        }
    }
}
