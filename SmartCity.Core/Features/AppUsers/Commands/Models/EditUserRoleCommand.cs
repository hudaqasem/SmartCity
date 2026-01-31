using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Models
{
    public class EditUserRoleCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }
}
