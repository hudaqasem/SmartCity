using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Roles.Commands.Models
{
    public class AddRoleCommand : IRequest<Response<string>>
    {
        public string RoleName { get; set; }
    }
}
