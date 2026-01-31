using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.ResponseUnits.Commands.Models;
using SmartCity.AppCore.Features.ResponseUnits.Queries.Models;
using System.Security.Claims;

namespace SmartCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ResponseUnit")]
    public class ResponseUnitController : AppControllerBase
    {
        private string GetCurrentUserId()
        {
            return User.FindFirst("Id")?.Value ??
                   User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }





        [HttpGet("my-assignments")]
        public async Task<IActionResult> GetMyAssignments()
        {
            var userId = GetCurrentUserId();
            var response = await Mediator.Send(new GetUnitAssignmentsQuery()
            {
                UserId = userId
            });
            return NewResult(response);

        }


        [HttpPost("accept")]
        public async Task<IActionResult> AcceptAssignment([FromBody] AcceptAssignmentCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        [HttpPost("reject")]
        public async Task<IActionResult> RejectAssignment([FromBody] RejectAssignmentCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Mark arrival at incident scene
        /// Body: { "incidentId": 123, "unitId": 456 }
        /// </summary>
        [HttpPost("arrive")]
        public async Task<IActionResult> ArriveAtScene([FromBody] ArriveAtSceneCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Complete assignment and resolve incident
        /// Body: { "incidentId": 123, "unitId": 456, "notes": "Fire extinguished" }
        /// </summary>
        [HttpPost("complete")]
        public async Task<IActionResult> CompleteAssignment([FromBody] CompleteAssignmentCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

    }
}
