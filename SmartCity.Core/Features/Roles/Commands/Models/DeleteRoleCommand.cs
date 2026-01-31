using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Roles.Commands.Models
{
    public class DeleteRoleCommand : IRequest<Response<string>>
    {
        public string Id { get; set; }
        public DeleteRoleCommand(string id)
        {
            Id = id;
        }
    }
}
