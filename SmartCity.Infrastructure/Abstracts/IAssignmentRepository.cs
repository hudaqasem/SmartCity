using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Abstracts
{
    public interface IAssignmentRepository : IGenericRepositoryAsync<Assignment>
    {
        Task<List<Assignment>> GetAssignmentsByIncidentIdAsync(int incidentId);
        Task<List<Assignment>> GetAssignmentsByUnitIdAsync(int unitId);
        Task<Assignment> GetByCompositeKeyAsync(int incidentId, int unitId);
        Task<List<Assignment>> GetActiveAssignmentsByUnitAsync(int unitId);
        Task<List<Assignment>> GetAllAssignmentsByUnitAsync(int unitId);
        Task<Assignment> GetActiveAssignmentByIncidentAsync(int incidentId);
    }
}
