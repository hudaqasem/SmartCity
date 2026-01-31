using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Queries.Results;

namespace SmartCity.AppCore.Features.Incidents.Queries.Models
{
    public class GetMyIncidentsQuery : IRequest<Response<List<GetMyIncidentResponse>>>
    {
        public string UserId { get; set; }
    }
}
