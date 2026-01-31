using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Repositories
{
    public class AssignmentRepository : GenericRepositoryAsync<Assignment>,
        IAssignmentRepository
    {
        private readonly DbSet<Assignment> _assignments;

        public AssignmentRepository(ApplicationDbContext context)
            : base(context)
        {
            _assignments = context.Set<Assignment>();
        }

        public async Task<List<Assignment>> GetAssignmentsByIncidentIdAsync(int incidentId)
        {
            return await _assignments
                .Where(a => a.IncidentId == incidentId)
                .Include(a => a.Unit)
                .Include(a => a.Incident)
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetAssignmentsByUnitIdAsync(int unitId)
        {
            return await _assignments
                .Where(a => a.UnitId == unitId)
                .Include(a => a.Unit)
                .Include(a => a.Incident)
                .ToListAsync();
        }


        public async Task<Assignment> GetByCompositeKeyAsync(int incidentId, int unitId)
        {
            return await _assignments
                .Include(a => a.Incident)
                .Include(a => a.Unit)
                .FirstOrDefaultAsync(a =>
                    a.IncidentId == incidentId &&
                    a.UnitId == unitId);
        }

        public async Task<List<Assignment>> GetActiveAssignmentsByUnitAsync(int unitId)
        {
            return await _assignments
                .Include(a => a.Incident)
                    .ThenInclude(i => i.ReportedByUser)
                .Where(a => a.UnitId == unitId &&
                           (a.Status == AssignmentStatus.Assigned ||
                            a.Status == AssignmentStatus.Accepted))
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetAllAssignmentsByUnitAsync(int unitId)
        {
            return await _assignments
                .Include(a => a.Incident)
                    .ThenInclude(i => i.ReportedByUser)
                .Where(a => a.UnitId == unitId)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        public async Task<Assignment> GetActiveAssignmentByIncidentAsync(int incidentId)
        {
            return await _assignments
                .Include(a => a.Unit)
                .Include(a => a.Incident)
                .Where(a => a.IncidentId == incidentId &&
                           (a.Status == AssignmentStatus.Assigned ||
                            a.Status == AssignmentStatus.Accepted))
                .OrderByDescending(a => a.AssignedAt)
                .FirstOrDefaultAsync();
        }


    }
}
