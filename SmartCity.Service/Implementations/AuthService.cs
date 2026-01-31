using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartCity.Domain.Helper;
using SmartCity.Domain.Models;
using SmartCity.Domain.Results;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Service.Abstracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartCity.Service.Implementations
{
    public class AuthService : IAuthService
    {
        #region Fields
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Constructor
        public AuthService(
            JwtSettings jwtSettings,
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = jwtSettings;
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;

        }

        #endregion

        #region LOGIN – Generate Tokens

        public async Task<JwtAuthResult> GetJWTToken(ApplicationUser user)
        {
            var (jwtToken, accessToken) = await GenerateJWTToken(user);

            var refreshToken = GenerateNewRefreshToken(user.UserName);

            var refreshRecord = new UserRefreshToken
            {
                UserId = user.Id,
                JwtId = jwtToken.Id,
                RefreshToken = refreshToken.TokenString,
                AddedTime = DateTime.UtcNow,
                ExpiryDate = refreshToken.ExpireAt,
                IsUsed = false,
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshRecord);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                refreshToken = refreshToken
            };
        }

        private async Task<(JwtSecurityToken, string)> GenerateJWTToken(ApplicationUser user)
        {
            var claims = await GetClaims(user);

            // Add JTI Claim
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var jwtToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.AccessTokenExpireDate),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                     SecurityAlgorithms.HmacSha256)
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return (jwtToken, accessToken);
        }

        private RefreshToken GenerateNewRefreshToken(string username)
        {
            return new RefreshToken
            {
                TokenString = GenerateSecureRefreshToken(),
                ExpireAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDate)
            };
        }

        private string GenerateSecureRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(randomBytes);
        }

        #endregion

        #region CLAIMS

        public async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            // user is ==> ["Admin", "Manager"]
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim("UserName", user.UserName),
                new Claim("Email", user.Email),
                new Claim("PhoneNumber", user.PhoneNumber ?? "")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            return claims;
        }

        #endregion

        #region READ TOKEN

        public JwtSecurityToken ReadJWTToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentNullException(nameof(accessToken));

            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(accessToken);
        }

        #endregion

        #region VALIDATE OLD REFRESH TOKEN

        public async Task<(string, DateTime?)> ValidateDetails(
            JwtSecurityToken jwtToken,
            string accessToken,
            string refreshToken)
        {
            if (jwtToken == null ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return ("AlgorithmIsWrong", null);
            }

            if (jwtToken.ValidTo > DateTime.UtcNow)
            {
                return ("TokenIsNotExpired", null);
            }

            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;

            if (userId == null)
                return ("UserIdNotFoundInToken", null);

            var tokenRecord = await _refreshTokenRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.RefreshToken == refreshToken &&
                    x.UserId == userId);

            if (tokenRecord == null)
                return ("RefreshTokenIsNotFound", null);

            if (tokenRecord.IsUsed)
                return ("RefreshTokenIsUsed", null);

            if (tokenRecord.IsRevoked)
                return ("RefreshTokenIsRevoked", null);

            if (tokenRecord.ExpiryDate <= DateTime.UtcNow)
            {
                tokenRecord.IsRevoked = true;
                await _refreshTokenRepository.UpdateAsync(tokenRecord);
                return ("RefreshTokenIsExpired", null);
            }

            if (tokenRecord.JwtId != jwtToken.Id)
                return ("JwtIdMismatch", null);

            return (userId, tokenRecord.ExpiryDate);
        }

        #endregion

        #region GENERATE NEW TOKEN (REFRESH FLOW)

        public async Task<JwtAuthResult> GetRefreshToken(
            ApplicationUser user,
            JwtSecurityToken oldJwtToken,
            DateTime? oldRefreshExpiry,
            string oldRefreshToken)
        {
            var oldRecord = await _refreshTokenRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(x =>
                    x.RefreshToken == oldRefreshToken &&
                    x.UserId == user.Id);

            if (oldRecord != null)
            {
                oldRecord.IsUsed = true;
                oldRecord.IsRevoked = false;
                await _refreshTokenRepository.UpdateAsync(oldRecord);
            }

            var (newJwt, newAccessToken) = await GenerateJWTToken(user);

            var newRefresh = GenerateNewRefreshToken(user.UserName);

            var newRecord = new UserRefreshToken
            {
                UserId = user.Id,
                JwtId = newJwt.Id,
                RefreshToken = newRefresh.TokenString,
                AddedTime = DateTime.UtcNow,
                ExpiryDate = newRefresh.ExpireAt,
                IsUsed = false,
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(newRecord);

            return new JwtAuthResult
            {
                AccessToken = newAccessToken,
                refreshToken = newRefresh
            };
        }

        #endregion

        #region LOGOUT - Revoke Token

        public async Task<string> RevokeToken(string accessToken, string refreshToken)
        {
            // Remove Bearer if sent
            accessToken = accessToken.Replace("Bearer ", "");

            var jwtToken = ReadJWTToken(accessToken);

            // Get userId from token
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userId))
                return "InvalidToken";

            // Find the refresh token record
            var tokenRecord = await _refreshTokenRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(x =>
                    x.RefreshToken == refreshToken &&
                    x.UserId == userId);

            if (tokenRecord == null)
                return "RefreshTokenNotFound";

            // Revoke the token
            tokenRecord.IsRevoked = true;
            tokenRecord.IsUsed = true;
            await _refreshTokenRepository.UpdateAsync(tokenRecord);

            return "Success";
        }

        // Revoke all tokens for the current user (Logout from all devices)
        //public async Task<string> RevokeAllUserTokens(string userId)
        //{
        //    // Get all active tokens for this user
        //    var userTokens = await _refreshTokenRepository.GetTableAsTracking()
        //        .Where(x => x.UserId == userId && !x.IsRevoked)
        //        .ToListAsync();

        //    if (!userTokens.Any())
        //        return "NoActiveTokens";

        //    // Revoke all tokens
        //    foreach (var token in userTokens)
        //    {
        //        token.IsRevoked = true;
        //        token.IsUsed = true;
        //    }

        //    await _refreshTokenRepository.UpdateRangeAsync(userTokens);

        //    return "Success";
        //}

        #endregion






    }
}
