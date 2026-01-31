using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Repositories
{
    public class IncidentRepository : GenericRepositoryAsync<Incident>,
        IIncidentRepository
    {
        #region Fields

        private readonly DbSet<Incident> _incidents;

        #endregion

        #region Constructors
        public IncidentRepository(ApplicationDbContext _context) : base(_context)
        {
            _incidents = _context.Set<Incident>();
        }

        #endregion

        #region Handles Functions

        public async Task<List<Incident>> GetWaitingIncidentsAsync()
        {
            return await _incidents
                .Where(i => i.Status == IncidentStatus.WaitingForUnit)
                .ToListAsync();
        }

        public Task<List<Incident>> GetWaitingForUnitAsync()
        {
            return _incidents
                .Where(i => i.Status == IncidentStatus.WaitingForUnit)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
        }

        #endregion
    }
}
