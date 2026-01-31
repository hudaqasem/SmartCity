using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.Domain.Results;

namespace SmartCity.AppCore.Features.Auth.Commands.Models
{
    public class RefreshTokenCommand : IRequest<Response<JwtAuthResult>>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
