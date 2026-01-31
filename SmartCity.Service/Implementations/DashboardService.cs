using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Results;
using SmartCity.Infrastructure.Data;
using SmartCity.Service.Abstracts;

namespace SmartCity.Service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _db;

        public DashboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(
            DateTime? startDate,
            DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow.Date;
            var end = endDate ?? DateTime.UtcNow.Date.AddDays(1);

            var incidents = await _db.Incidents
                .Where(i => i.CreatedAt >= start && i.CreatedAt < end)
                .ToListAsync();

            var units = await _db.ResponseUnits.ToListAsync();

            var assignments = await _db.Assignments
                .Include(a => a.Incident)
                .Where(a => a.AssignedAt >= start && a.AssignedAt < end)
                .ToListAsync();

            // Calculate response times
            var completedAssignments = assignments
                .Where(a => a.Status == AssignmentStatus.Completed)
                .ToList();

            var avgResponseTime = completedAssignments.Any()
                ? completedAssignments.Average(a => a.ResponseTime.Value.TotalMinutes)
                : 0;

            var avgCompletionTime = completedAssignments.Any()
                ? completedAssignments.Average(a => a.CompletionTime?.TotalMinutes ?? 0)
                : 0;

            // Incidents by type
            var incidentsByType = incidents
                .GroupBy(i => i.Type)
                .Select(g => new IncidentTypeStats
                {
                    Type = g.Key.ToString(),
                    Total = g.Count(),
                    Active = g.Count(i => i.Status != IncidentStatus.Resolved),
                    Resolved = g.Count(i => i.Status == IncidentStatus.Resolved)
                })
                .ToList();

            // Units by type
            var unitsByType = units
                .GroupBy(u => u.Type)
                .Select(g => new UnitTypeStats
                {
                    Type = g.Key.ToString(),
                    Total = g.Count(),
                    Available = g.Count(u => u.Status == UnitStatus.Available),
                    Busy = g.Count(u => u.Status == UnitStatus.EnRoute || u.Status == UnitStatus.OnScene)
                })
                .ToList();

            return new DashboardSummaryResponse
            {
                // Incidents
                TotalIncidents = incidents.Count,
                ActiveIncidents = incidents.Count(i => i.Status != IncidentStatus.Resolved),
                WaitingForUnit = incidents.Count(i => i.Status == IncidentStatus.WaitingForUnit),
                InProgressIncidents = incidents.Count(i => i.Status == IncidentStatus.InProgress || i.Status == IncidentStatus.OnScene),
                ResolvedToday = incidents.Count(i => i.Status == IncidentStatus.Resolved),

                // Units
                TotalUnits = units.Count,
                AvailableUnits = units.Count(u => u.Status == UnitStatus.Available),
                BusyUnits = units.Count(u => u.Status == UnitStatus.EnRoute || u.Status == UnitStatus.OnScene),
                OfflineUnits = units.Count(u => u.Status == UnitStatus.Offline),

                // Performance
                AverageResponseTime = Math.Round(avgResponseTime, 2),
                AverageCompletionTime = Math.Round(avgCompletionTime, 2),

                // Breakdowns
                IncidentsByType = incidentsByType,
                UnitsByType = unitsByType
            };
        }
    }
}
