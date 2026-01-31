using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Roles.Commands.Models
{
    public class EditRoleCommand : IRequest<Response<string>>
    {
        public string Id { get; set; }
        public string NewRoleName { get; set; }
    }
}
