using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Models
{
    public class EditUserCommand : IRequest<Response<string>>
    {

        public string Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
