using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.AppUsers.Queries.Results;

namespace SmartCity.AppCore.Features.AppUsers.Queries.Models
{
    public class GetUserRolesQuery : IRequest<Response<GetUserRolesResponse>>
    {
        public string UserId { get; set; }

        public GetUserRolesQuery(string userId)
        {
            UserId = userId;
        }
    }
}
