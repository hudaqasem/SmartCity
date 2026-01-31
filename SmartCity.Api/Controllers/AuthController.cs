using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.Auth.Commands.Models;

namespace SmartCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {

        #region Register

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        #endregion

        #region Login

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }



        #endregion

        #region Logout
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        // Revoke all tokens for the current user (Logout from all devices)

        //[Authorize]
        //[HttpPost("LogoutAll")]
        //public async Task<IActionResult> LogoutAll()
        //{
        //    // Get userId from JWT claims
        //    var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //        return BadRequest("Invalid token");

        //    var result = await _authService.RevokeAllUserTokens(userId);

        //    if (result == "Success")
        //        return Ok(new { succeeded = true, message = "Logged out from all devices" });

        //    return BadRequest(new { succeeded = false, message = "Logout failed" });
        //}
        #endregion



    }
}
