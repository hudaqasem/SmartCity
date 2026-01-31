using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCity.Service.Abstracts;
using SmartCity.Service.Implementations;

namespace SmartCity.Service
{
    public static class ModuleServiceDependancies
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services)
        {
            services.AddTransient<IResponseUnitService, ResponseUnitService>();
            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<IIncidentService, IncidentService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRoleManagementService, RoleManagementService>();
            services.AddTransient<IUnitDistanceService, UnitDistanceService>();
            services.AddTransient<IUnitAssignmentService, UnitAssignmentService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IAdminResponseUnitService, AdminResponseUnitService>();

            // ═══════════════════════════════════════════════════════
            // AI Detection Service with Resilience
            // ═══════════════════════════════════════════════════════
            services.AddHttpClient<IAIDetectionService, AIDetectionService>()
                .ConfigureHttpClient((sp, client) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var baseUrl = config["AIService:BaseUrl"] ?? "http://localhost:5000";
                    var timeout = config.GetValue<int>("AIService:Timeout", 30);

                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                })
                .AddStandardResilienceHandler(options =>
                {
                    // Retry configuration
                    options.Retry.MaxRetryAttempts = 3;
                    options.Retry.Delay = TimeSpan.FromSeconds(2);
                    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
                    options.Retry.UseJitter = true;

                    // Circuit breaker configuration
                    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
                    options.CircuitBreaker.FailureRatio = 0.5;
                    options.CircuitBreaker.MinimumThroughput = 5;
                    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

                    // Timeout configuration
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);
                });

            return services;
        }
    }
}