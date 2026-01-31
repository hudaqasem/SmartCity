using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Models
{
    public class DeleteUserCommand : IRequest<Response<string>>
    {
        public string Id { get; set; }
        public DeleteUserCommand(string id)
        {
            Id = id;
        }
    }
}
