using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.EntityFrameworkCore;
using SmartCity.AppCore;
using SmartCity.AppCore.Middleware;
using SmartCity.Domain.Helper;
using SmartCity.Infrastructure;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Seeder;
using SmartCity.Service;
using SmartCity.Service.Abstracts;
using SmartCity.Service.Implementations;
using SmartCity.Service.Implementations.Firebase;
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

            // frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });


            #region Dependancy Injection

            builder.Services.AddInfrastructureDependancies()
                .AddServiceDependancies()
                .AddCoreDependancies()
                .AddServiceRegiteration(builder.Configuration);


            #endregion

            builder.Services.Configure<FirebaseRealtimeConfig>(
            builder.Configuration.GetSection("FirebaseRealtime")
);
            builder.Services.AddSingleton<IFirebaseAccessTokenProvider, GoogleAccessTokenProvider>();

            builder.Services.AddHttpClient("FirebaseRealtime", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddHostedService<SmartCity.Service.Implementations.FirebaseRealtimePollingService>();

            //builder.Services.AddHostedService<FirebaseRealtimePollingService>();

            var app = builder.Build();

            // Run Identity Seed
            using (var scope = app.Services.CreateScope())
            {

                var serviceProvider = scope.ServiceProvider;
                // --- ������� ��� ��� ���� ---
               // var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
               // context.Database.Migrate(); // �� ����� Update-Database ��������� ��� �� ������ ����
                                            // ----------------------------
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
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}