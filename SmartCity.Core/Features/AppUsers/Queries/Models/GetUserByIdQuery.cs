using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.AppUsers.Queries.Results;

namespace SmartCity.AppCore.Features.AppUsers.Queries.Models
{
    public class GetUserByIdQuery : IRequest<Response<GetUserDetailsResponse>>
    {
        public string Id { get; set; }
        public GetUserByIdQuery(string id)
        {
            Id = id;
        }
    }
}
