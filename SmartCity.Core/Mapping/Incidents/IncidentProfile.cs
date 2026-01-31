using AutoMapper;

namespace SmartCity.AppCore.Mapping.Incidents
{
    public partial class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            GetIncidentListMapping();
            CreateIncidentCommandMapping();
            GetMyIncidentMapping();
            GetIncidentById();
            GetIncidentMapping();
        }
    }
}
