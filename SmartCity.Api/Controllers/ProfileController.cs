using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;
using SmartCity.AppCore.Features.AppUsers.Queries.Models;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : AppControllerBase
{
    private string GetCurrentUserId()
    {
        return User.FindFirst("Id")?.Value ??
               User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetCurrentUserId();
        var response = await Mediator.Send(new GetUserByIdQuery(userId));
        return NewResult(response);
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> EditMyProfile([FromBody] EditMyProfileCommand command)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { succeeded = false, message = "User not authenticated" });

        // Set the Id from the authenticated user
        command.Id = userId;

        var response = await Mediator.Send(command);
        return NewResult(response);
    }

    [HttpPut("ChangePassword")]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeUserPasswordCommand command)
    {
        command.Id = GetCurrentUserId();
        var response = await Mediator.Send(command);
        return NewResult(response);
    }
}
