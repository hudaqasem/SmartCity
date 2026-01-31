using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.ResponseUnits.Queries.Results;

namespace SmartCity.AppCore.Features.ResponseUnits.Queries.Models
{
    public class GetUnitAssignmentsQuery : IRequest<Response<List<UnitAssignmentResponse>>>
    {
        public string UserId { get; set; }
        public bool ActiveOnly { get; set; } = true; // Show only active assignments by default
    }
}
