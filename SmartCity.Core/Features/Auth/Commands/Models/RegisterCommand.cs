using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Auth.Commands.Results;

namespace SmartCity.AppCore.Features.Auth.Commands.Models
{
    public class RegisterCommand : IRequest<Response<RegisterResponse>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }

}
