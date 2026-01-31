using SmartCity.Infrastructure.Abstracts;
using SmartCity.Service.Abstracts;

namespace SmartCity.Service.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository adminRepository;

        public AdminService(IAdminRepository _adminRepository)
        {
            adminRepository = _adminRepository;
        }

        public async Task<(int totalIncidents, int newIncidents, int inProgressIncidents, int resolvedIncidents, int activeUnits, int totalCitizens)>
            GetDashboardSummaryAsync()
        {
            return await adminRepository.GetDashboardSummaryAsync();
        }
    }
}
