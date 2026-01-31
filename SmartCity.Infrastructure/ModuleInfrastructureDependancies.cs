using Microsoft.Extensions.DependencyInjection;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Generics;
using SmartCity.Infrastructure.Repositories;

namespace SmartCity.Infrastructure
{
    public static class ModuleInfrastructureDependancies
    {
        public static IServiceCollection AddInfrastructureDependancies(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));

            services.AddTransient<IResponseUnitRepository, ResponseUnitRepository>();
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            services.AddTransient<IIncidentRepository, IncidentRepository>();
            services.AddTransient<IAdminRepository, AdminRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IAssignmentRepository, AssignmentRepository>();
            return services;
        }

    }
}
