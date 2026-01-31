using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Models
{
    public class CreateUserCommand : IRequest<Response<string>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}
