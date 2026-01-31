using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Abstracts
{
    public interface IRefreshTokenRepository : IGenericRepositoryAsync<UserRefreshToken>
    {

    }
}
