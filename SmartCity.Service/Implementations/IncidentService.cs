using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Service.Abstracts;

namespace SmartCity.Service.Implementations
{
    public class IncidentService : IIncidentService
    {
        #region Fields

        private readonly IIncidentRepository incidentRepository;

        #endregion

        #region Constructors
        public IncidentService(IIncidentRepository _incidentRepository)
        {
            incidentRepository = _incidentRepository;
        }


        #endregion

        #region Handles Functions

        public async Task<List<Incident>> GetIncidentsAsync()
        {
            return await incidentRepository.GetAllAsync();
        }

        //public async Task<Incident> ReportIncidentAsync(Incident incident)
        //{
        //    incident.Status = IncidentStatus.New;
        //    return await incidentRepository.AddAsync(incident); ;
        //}


        public async Task<Incident> ReportIncidentAsync(Incident incident)
        {
            // save incident initially
            incident.Status = IncidentStatus.New;
            var createdIncident = await incidentRepository.AddAsync(incident);

            return createdIncident;
        }


        public async Task<List<Incident>> GetIncidentByUserIdAsync(string id)
        {
            return await incidentRepository.GetTableNoTracking()
                .Where(i => i.ReportedByUserId == id)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<Incident?> GetByIdAsync(int id)
        {
            return await incidentRepository.GetTableNoTracking()
               .Include(i => i.Assignments
                   .OrderByDescending(a => a.AssignedAt))
                   .ThenInclude(a => a.Unit)
               .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Incident>> GetAllFilteredAsync(IncidentStatus? status, string? type, DateTime? from, DateTime? to)
        {
            var incidents = await incidentRepository.GetAllAsync();

            if (status.HasValue)
                incidents = incidents.Where(i => i.Status == status.Value).ToList();

            if (!string.IsNullOrEmpty(type))
                incidents = incidents.Where(i => i.Type == type).ToList();

            if (from.HasValue)
                incidents = incidents.Where(i => i.CreatedAt >= from.Value).ToList();

            if (to.HasValue)
                incidents = incidents.Where(i => i.CreatedAt <= to.Value).ToList();

            return incidents;
        }

        #endregion
    }
}
