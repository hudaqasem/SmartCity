using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.Admin.Commands.Models;
using SmartCity.AppCore.Features.Admin.Queries.Models;

namespace SmartCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MonitoringController : AppControllerBase
    {
        /// <summary>
        /// Get all incidents waiting for response units
        /// </summary>
        [HttpGet("waiting-incidents")]
        public async Task<IActionResult> GetWaitingIncidents()
        {
            var response = await Mediator.Send(new GetWaitingIncidentsQuery());
            return NewResult(response);
        }

        /// <summary>
        /// Get available units (optionally filter by type)
        /// Example: ?type=Fire or ?type=Police
        /// </summary>
        [HttpGet("available-units")]
        public async Task<IActionResult> GetAvailableUnits([FromQuery] string type = null)
        {
            var query = new GetAvailableUnitsQuery { Type = type };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Get nearest units for a specific incident
        /// </summary>
        [HttpGet("nearest-units/{incidentId}")]
        public async Task<IActionResult> GetNearestUnits(int incidentId, [FromQuery] int topCount = 5)
        {
            var query = new GetNearestUnitsQuery
            {
                IncidentId = incidentId,
                TopCount = topCount
            };

            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Manually assign a specific unit to an incident
        /// </summary>
        [HttpPost("manual-assign")]
        public async Task<IActionResult> ManualAssign([FromBody] ManualAssignCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
