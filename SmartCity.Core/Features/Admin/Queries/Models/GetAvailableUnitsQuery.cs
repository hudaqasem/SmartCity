using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Admin.Queries.Results;

namespace SmartCity.AppCore.Features.Admin.Queries.Models
{
    public class GetAvailableUnitsQuery : IRequest<Response<List<UnitAvailabilityResponse>>>
    {
        public string Type { get; set; } // "Fire", "Police", "Medical", etc.
    }
}
