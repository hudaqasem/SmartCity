
using SmartCity.Domain.Results;

namespace SmartCity.Service.Abstracts
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetDashboardSummaryAsync(DateTime? startDate, DateTime? endDate);
    }
}
