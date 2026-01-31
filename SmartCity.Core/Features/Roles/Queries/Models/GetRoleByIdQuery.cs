using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Roles.Queries.Models
{
    public class GetRoleByIdQuery : IRequest<Response<IdentityRole>>
    {
        public string Id { get; set; }
        public GetRoleByIdQuery(string id)
        {
            Id = id;
        }
    }
}
