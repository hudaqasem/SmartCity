using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Enums;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;

namespace SmartCity.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext context;

        public AdminRepository(ApplicationDbContext db)
        {
            context = db;
        }

        public async Task<(int totalIncidents, int newIncidents, int inProgressIncidents, int resolvedIncidents, int activeUnits, int totalCitizens)>
            GetDashboardSummaryAsync()
        {
            var totalIncidents = await context.Incidents.CountAsync();
            var newIncidents = await context.Incidents.CountAsync(i => i.Status == IncidentStatus.New);
            var inProgressIncidents = await context.Incidents.CountAsync(i => i.Status == IncidentStatus.InProgress);
            var resolvedIncidents = await context.Incidents.CountAsync(i => i.Status == IncidentStatus.Resolved);

            var activeUnits = await context.ResponseUnits.CountAsync(u => u.IsActive);
            var totalCitizens = 5;

            return (totalIncidents, newIncidents, inProgressIncidents, resolvedIncidents, activeUnits, totalCitizens);
        }
    }
}
