using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.Domain.Results;

namespace SmartCity.AppCore.Features.Auth.Commands.Models
{
    public class SignInCommand : IRequest<Response<JwtAuthResult>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
