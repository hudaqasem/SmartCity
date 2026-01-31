using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Auth.Commands.Models
{
    public class LogoutCommand : IRequest<Response<string>>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}