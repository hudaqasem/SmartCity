namespace SmartCity.Infrastructure.Abstracts
{
    public interface IAdminRepository
    {
        Task<(int totalIncidents, int newIncidents, int inProgressIncidents, int resolvedIncidents, int activeUnits, int totalCitizens)>
            GetDashboardSummaryAsync();
    }
}
