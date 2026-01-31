using Hangfire;
using Microsoft.EntityFrameworkCore;
using SmartCity.AppCore;
using SmartCity.AppCore.Middleware;
using SmartCity.Infrastructure;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Seeder;
using SmartCity.Service;
using SmartCity.Service.Abstracts;
using System.Text.Json.Serialization;

namespace SmartCity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Services.AddControllers();
            builder.Services.AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.Converters.Add(
                         new JsonStringEnumConverter()
                     );
                 });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("context")));


            // Hangfire
            builder.Services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(builder.Configuration.GetConnectionString("context"));
            });

            builder.Services.AddHangfireServer();


            #region Dependancy Injection

            builder.Services.AddInfrastructureDependancies()
                .AddServiceDependancies()
                .AddCoreDependancies()
                .AddServiceRegiteration(builder.Configuration);


            #endregion

            var app = builder.Build();

            // Run Identity Seed
            using (var scope = app.Services.CreateScope())
            {

                var serviceProvider = scope.ServiceProvider;

                // 1. Run Identity Seed (Roles and Admin User)
                IdentitySeed.SeedRolesAndAdmin(serviceProvider).Wait();

                // 2. Run Response Incident Seed (Units, and Test Incident)
                ResponseIncidentSeed.SeedInitialData(serviceProvider).Wait();
            }

            //HangfireDashboard
            app.UseHangfireDashboard("/hangfire");

            RecurringJob.AddOrUpdate<IUnitAssignmentService>(
                            "retry-waiting-incidents",
                         service => service.RetryAllWaitingAsync(),
                         "*/5 * * * *"
            );


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
