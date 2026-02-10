using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;
using SmartCity.AppCore.Features.AppUsers.Queries.Models;

namespace SmartCity.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AppUserController : AppControllerBase
    {



        #region Get All

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await Mediator.Send(new GetUserListQuery());
            return Ok(response);
        }
        #endregion

        #region Add 
        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        #endregion

        #region Delete
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            return NewResult(await Mediator.Send(new DeleteUserCommand(Id)));
        }

        #endregion

        #region Change Role

        [HttpPut("ChangeRole")]
        public async Task<IActionResult> ChangeUserRole([FromBody] EditUserRoleCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        #endregion

        #region Get User Roles
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            var response = await Mediator.Send(new GetUserRolesQuery(id));
            return NewResult(response);
        }
        #endregion

        #region Get by Id

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var response = await Mediator.Send(new GetUserByIdQuery(id));
            return NewResult(response);
        }
        #endregion

        #region Edit user & Change Password

        [HttpPut("Edit")]
        public async Task<IActionResult> EditUser([FromBody] EditUserCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        #endregion



    }
}