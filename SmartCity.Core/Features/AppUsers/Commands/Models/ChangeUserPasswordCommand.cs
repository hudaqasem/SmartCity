using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Models
{
    public class ChangeUserPasswordCommand : IRequest<Response<string>>
    {
        public string? Id { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public ChangeUserPasswordCommand(string id)
        {
            Id = id;
        }
    }
}
