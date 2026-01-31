using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Devices.Queries.Results;

namespace SmartCity.AppCore.Features.Devices.Queries.Models
{
    public class GetDeviceQuery : IRequest<Response<List<GetDeviceListResponse>>>
    {
    }
}
