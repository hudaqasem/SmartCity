using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.ResponseUnits.Queries.Results;

namespace SmartCity.AppCore.Features.ResponseUnits.Queries.Models
{
    public class GetResponseUnitQuery : IRequest<Response<List<GetUnitListResponse>>>
    {
    }
}
