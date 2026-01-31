using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Admin.Queries.Results;

namespace SmartCity.AppCore.Features.Admin.Queries.Models
{
    public class GetNearestUnitsQuery : IRequest<Response<List<NearestUnitResponse>>>
    {
        public int IncidentId { get; set; }
        public int TopCount { get; set; } = 5;
    }
}
