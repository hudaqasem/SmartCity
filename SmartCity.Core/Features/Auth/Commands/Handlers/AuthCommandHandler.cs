using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Auth.Commands.Models;
using SmartCity.AppCore.Features.Auth.Commands.Results;
using SmartCity.Domain.Models;
using SmartCity.Domain.Results;
using SmartCity.Service.Abstracts;
using System.Data;

namespace SmartCity.AppCore.Features.Auth.Commands.Handlers
{
    public class AuthCommandHandler : ResponseHandler,
     IRequestHandler<RegisterCommand, Response<RegisterResponse>>,
     IRequestHandler<SignInCommand, Response<JwtAuthResult>>,
     IRequestHandler<RefreshTokenCommand, Response<JwtAuthResult>>,
     IRequestHandler<LogoutCommand, Response<string>>
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        #endregion

        #region Constructor
        public AuthCommandHandler(
            UserManager<ApplicationUser> userManager,
            IAuthService authService,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _authService = authService;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        #endregion

        #region Register

        public async Task<Response<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<ApplicationUser>(request);
            user.UserName = request.Email;

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest<RegisterResponse>(errors);
            }

            // Assign Citizen Role auto
            await _userManager.AddToRoleAsync(user, "Citizen");

            var response = _mapper.Map<RegisterResponse>(user);

            return Created(response);
        }

        #endregion

        #region Sign In

        public async Task<Response<JwtAuthResult>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
                return BadRequest<JwtAuthResult>("User not found");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!signInResult.Succeeded)
                return BadRequest<JwtAuthResult>("Password is wrong");

            var result = await _authService.GetJWTToken(user);

            return Success(result);
        }

        #endregion

        #region Refresh Token

        public async Task<Response<JwtAuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Remove Bearer if sent
            var accessToken = request.AccessToken.Replace("Bearer ", "");

            var jwtToken = _authService.ReadJWTToken(accessToken);

            var (userIdOrMessage, expiryDate) =
                await _authService.ValidateDetails(jwtToken, accessToken, request.RefreshToken);

            // Handle errors
            if (userIdOrMessage is "AlgorithmIsWrong")
                return Unauthorized<JwtAuthResult>("Algorithm Is Wrong");

            if (userIdOrMessage is "TokenIsNotExpired")
                return Unauthorized<JwtAuthResult>("Token Is Not Expired");

            if (userIdOrMessage is "RefreshTokenIsNotFound")
                return Unauthorized<JwtAuthResult>("Refresh Token Not Found");

            if (userIdOrMessage is "RefreshTokenIsUsed")
                return Unauthorized<JwtAuthResult>("Refresh Token Already Used");

            if (userIdOrMessage is "RefreshTokenIsRevoked")
                return Unauthorized<JwtAuthResult>("Refresh Token Revoked");

            if (userIdOrMessage is "RefreshTokenIsExpired")
                return Unauthorized<JwtAuthResult>("Refresh Token Expired");

            if (userIdOrMessage is "JwtIdMismatch")
                return Unauthorized<JwtAuthResult>("JWT ID doesn't match");

            if (userIdOrMessage is "UserIdNotFoundInToken")
                return Unauthorized<JwtAuthResult>("Invalid token content");

            // Now userIdOrMessage contains actual userId
            var userId = userIdOrMessage;

            // Load user
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound<JwtAuthResult>("User not found");

            // Generate new token
            var result = await _authService.GetRefreshToken(
                user,
                jwtToken,
                expiryDate,
                request.RefreshToken
            );

            return Success(result);
        }

        #endregion

        #region Logout

        public async Task<Response<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest<string>("Access token and refresh token are required");

            var result = await _authService.RevokeToken(request.AccessToken, request.RefreshToken);

            return result switch
            {
                "Success" => Success<string>("Logged out successfully"),
                "InvalidToken" => BadRequest<string>("Invalid access token"),
                "RefreshTokenNotFound" => BadRequest<string>("Refresh token not found or already revoked"),
                _ => BadRequest<string>("Logout failed")
            };
        }

        #endregion

    }

}
