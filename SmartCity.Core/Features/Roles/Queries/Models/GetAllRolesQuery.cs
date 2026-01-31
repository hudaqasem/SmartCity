using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Roles.Queries.Models
{
    public class GetAllRolesQuery : IRequest<Response<List<IdentityRole>>>
    {

    }
}
