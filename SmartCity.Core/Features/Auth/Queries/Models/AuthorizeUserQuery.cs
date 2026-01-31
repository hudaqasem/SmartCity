using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Auth.Queries.Models
{
    public class AuthorizeUserQuery : IRequest<Response<string>>
    {
        public string AccessToken { get; set; }
    }
}
