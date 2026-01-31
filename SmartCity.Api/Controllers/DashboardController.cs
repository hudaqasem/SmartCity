using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.Dashboard.Queries.Models;

namespace SmartCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : AppControllerBase
    {
        /// <summary>
        /// Get comprehensive dashboard statistics
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = new GetDashboardSummaryQuery
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Get real-time statistics (last 24 hours)
        /// </summary>
        [HttpGet("realtime")]
        public async Task<IActionResult> GetRealtime()
        {
            var query = new GetDashboardSummaryQuery
            {
                StartDate = DateTime.UtcNow.AddHours(-24),
                EndDate = DateTime.UtcNow
            };

            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Get weekly statistics
        /// </summary>
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeekly()
        {
            var query = new GetDashboardSummaryQuery
            {
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow
            };

            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Get monthly statistics
        /// </summary>
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly()
        {
            var query = new GetDashboardSummaryQuery
            {
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow
            };

            var response = await Mediator.Send(query);
            return NewResult(response);
        }
    }
}
