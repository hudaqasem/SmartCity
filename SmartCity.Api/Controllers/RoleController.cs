using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.Roles.Commands.Models;
using SmartCity.AppCore.Features.Roles.Queries.Models;

namespace SmartCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : AppControllerBase
    {
        [HttpPost("Add")]
        public async Task<IActionResult> Create([FromForm] AddRoleCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] EditRoleCommand command)
        {
            return NewResult(await Mediator.Send(command));

        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return NewResult(await Mediator.Send(new DeleteRoleCommand(id)));
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return NewResult(await Mediator.Send(new GetAllRolesQuery()));

        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return NewResult(await Mediator.Send(new GetRoleByIdQuery(id)));
        }


    }
}
