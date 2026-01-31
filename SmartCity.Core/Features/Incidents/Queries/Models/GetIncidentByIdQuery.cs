using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Queries.Results;

namespace SmartCity.AppCore.Features.Incidents.Queries.Models
{
    public class GetIncidentByIdQuery : IRequest<Response<GetIncidentDetailsResponse>>
    {
        public int Id { get; set; }
    }
}
