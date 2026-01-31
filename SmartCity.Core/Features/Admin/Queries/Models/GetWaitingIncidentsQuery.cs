using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Admin.Queries.Results;

namespace SmartCity.AppCore.Features.Admin.Queries.Models
{
    public class GetWaitingIncidentsQuery : IRequest<Response<List<WaitingIncidentResponse>>>
    {
    }
}
