using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin;
using SmartCity.AppCore.Features.ResponseUnits.Queries.Models;

[Route("api/admin/response-units")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ResponseUnitAdminController : AppControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateResponseUnitCommand command)
    {
        var response = await Mediator.Send(command);
        return NewResult(response);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] EditResponseUnitCommand command)
    {
        var response = await Mediator.Send(command);
        return NewResult(response);
    }


    [HttpPut("status")]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeUnitStatusCommand command)
    {
        var response = await Mediator.Send(command);
        return NewResult(response);
    }

    [HttpPut("active")]
    public async Task<IActionResult> ToggleActive([FromBody] EnableDisableUnitCommand command)
    {
        var response = await Mediator.Send(command);
        return NewResult(response);
    }

    [HttpPut("assign-user")]
    public async Task<IActionResult> AssignUser(AssignUserToUnitCommand command)
    {
        var response = await Mediator.Send(command);
        return NewResult(response);
    }

    #region GetAll

    [HttpGet("List")]
    public async Task<IActionResult> GetAllResponseUnits()
    {
        var response = await Mediator.Send(new GetResponseUnitQuery());
        return Ok(response);
    }

    #endregion

}
