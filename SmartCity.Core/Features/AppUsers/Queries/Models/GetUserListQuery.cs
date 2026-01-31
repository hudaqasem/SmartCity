using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.AppUsers.Queries.Results;

namespace SmartCity.AppCore.Features.AppUsers.Queries.Models
{
    public class GetUserListQuery : IRequest<Response<List<GetUserListResponse>>>
    {

    }
}
