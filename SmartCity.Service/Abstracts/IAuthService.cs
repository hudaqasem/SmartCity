using SmartCity.Domain.Models;
using SmartCity.Domain.Results;
using System.IdentityModel.Tokens.Jwt;

namespace SmartCity.Service.Abstracts
{
    public interface IAuthService
    {

        public Task<JwtAuthResult> GetJWTToken(ApplicationUser user);
        public JwtSecurityToken ReadJWTToken(string accessToken);
        public Task<(string, DateTime?)> ValidateDetails(JwtSecurityToken jwtToken, string accessToken, string refreshToken);
        public Task<JwtAuthResult> GetRefreshToken(ApplicationUser user, JwtSecurityToken jwtToken, DateTime? expiryDate, string refreshToken);

        //log out
        Task<string> RevokeToken(string accessToken, string refreshToken);
        //Task<string> RevokeAllUserTokens(string userId);
    }
}
