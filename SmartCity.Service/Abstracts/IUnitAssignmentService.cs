using SmartCity.Domain.Models;

namespace SmartCity.Service.Abstracts
{
    public interface IUnitAssignmentService
    {
        Task<bool> AutoAssignUnitAsync(Incident incident);
        Task<bool> RetryAssignAsync(int incidentId);
        Task<int> RetryAllWaitingAsync();
        Task<bool> ManualAssignAsync(int incidentId, int unitId);

        //********************
        Task<string> AcceptAsync(int incidentId, int unitId);
        Task<string> RejectAsync(int incidentId, int unitId, string reason);
        Task<string> ArriveAsync(int incidentId, int unitId);
        Task<string> CompleteAsync(int incidentId, int unitId, string notes);
    }
}
