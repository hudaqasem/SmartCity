using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepositoryAsync<UserRefreshToken>, IRefreshTokenRepository
    {
        #region Fields
        private DbSet<UserRefreshToken> userRefreshToken;
        #endregion

        #region Constructors
        public RefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            userRefreshToken = dbContext.Set<UserRefreshToken>();
        }
        #endregion

        #region Handle Functions

        #endregion
    }
}
