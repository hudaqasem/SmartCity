using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;

namespace SmartCity.Service.Abstracts
{
    public interface IIncidentService
    {
        Task<List<Incident>> GetIncidentsAsync();
        Task<Incident> ReportIncidentAsync(Incident incident);
        Task<Incident?> GetByIdAsync(int id);
        Task<List<Incident>> GetIncidentByUserIdAsync(string id);
        Task<List<Incident>> GetAllFilteredAsync(IncidentStatus? status, string? type, DateTime? from, DateTime? to);

    }
}
