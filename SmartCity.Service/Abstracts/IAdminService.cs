
namespace SmartCity.Service.Abstracts
{
    public interface IAdminService
    {
        Task<(int totalIncidents, int newIncidents, int inProgressIncidents, int resolvedIncidents, int activeUnits, int totalCitizens)>
           GetDashboardSummaryAsync();
    }
}
