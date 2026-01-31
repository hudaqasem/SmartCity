using Microsoft.Extensions.DependencyInjection;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Data;

namespace SmartCity.Infrastructure.Seeder
{
    public static class ResponseIncidentSeed
    {
        public static async Task SeedInitialData(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Response Units
            if (!context.ResponseUnits.Any())
            {
                await SeedResponseUnitsAsync(context);
            }

            // Seed Test Incidents
            if (!context.Incidents.Any())
            {
                await SeedTestIncidentsAsync(context);
            }

        }

        private static async Task SeedResponseUnitsAsync(ApplicationDbContext context)
        {
            var units = new List<ResponseUnit>
            {
                // Fire Units
                new()
                {
                    Name = "Fire Unit 1 - Downtown",
                    Type = "Fire",
                    Status = UnitStatus.Available,
                    IsActive = true,
                    Latitude = 30.0444,
                    Longitude = 31.2357,
                    Contact = "+201001234567"
                },
                new()
                {
                    Name = "Fire Unit 2 - Nasr City",
                    Type = "Fire",
                    Status = UnitStatus.Available,
                    IsActive = true,
                    Latitude = 30.0626,
                    Longitude = 31.3450,
                    Contact = "+201001234567"
                },
                new()
                {
                    Name = "Fire Unit 3 - Giza (BUSY)",
                    Type = "Fire",
                    Status = UnitStatus.EnRoute,
                    IsActive = true,
                    Latitude = 30.0131,
                    Longitude = 31.2089,
                    Contact = "+201001234567"
                },
                
                //  Medical Units
                new()
                {
                    Name = "Ambulance 1 - Maadi",
                    Type = "Medical",
                    Status = UnitStatus.Available,
                    IsActive = true,
                    Latitude = 29.9602,
                    Longitude = 31.2569,
                    Contact = "+201001234567"
                },
                new()
                {
                    Name = "Ambulance 2 - Heliopolis",
                    Type = "Medical",
                    Status = UnitStatus.Available,
                    IsActive = true,
                    Latitude = 30.0877,
                    Longitude = 31.3260,
                    Contact = "+201001234567"
                },
                new()
                {
                    Name = "Ambulance 3 - Mokattam (INACTIVE)",
                    Type = "Medical",
                    Status = UnitStatus.Available,
                    IsActive = false,
                    Latitude = 30.0084,
                    Longitude = 31.3250,
                    Contact = "+201001234567"
                },
                
                //Police Units
                new()
                {
                    Name = "Police Unit 1 - Zamalek",
                    Type = "Police",
                    Status = UnitStatus.Available,
                    IsActive = true,
                    Latitude = 30.0618,
                    Longitude = 31.2197,
                    Contact = "+201001234567"
                },
                new()
                {
                    Name = "Police Unit 2 - 6th October",
                    Type = "Police",
                    Status = UnitStatus.Available,
                    IsActive = true,
                    Latitude = 29.9526,
                    Longitude = 30.9286,
                    Contact = "+201001234567"
                }
            };

            await context.ResponseUnits.AddRangeAsync(units);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTestIncidentsAsync(ApplicationDbContext context)
        {
            var incidents = new List<Incident>
            {
                // Resolved Incident
                new()
                {
                    Type = "Fire",
                    Description = "Building fire in Downtown - Already resolved",
                    Status = IncidentStatus.Resolved,
                    Latitude = 30.0444,
                    Longitude = 31.2357,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                
                // In Progress Incident
                new()
                {
                    Type = "Medical",
                    Description = "Car accident on Ring Road",
                    Status = IncidentStatus.InProgress,
                    Latitude = 30.0626,
                    Longitude = 31.3450,
                    CreatedAt = DateTime.UtcNow.AddHours(-3)
                },
                
                // New Incident
                new()
                {
                    Type = "Police",
                    Description = "Theft reported in Zamalek area",
                    Status = IncidentStatus.New,
                    Latitude = 30.0618,
                    Longitude = 31.2197,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                },
                
                // Incident without coordinates (edge case)
                new()
                {
                    Type = "Fire",
                    Description = "Fire alarm - Location unknown",
                    Status = IncidentStatus.New,
                    Latitude = null,
                    Longitude = null,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-15)
                }
            };

            await context.Incidents.AddRangeAsync(incidents);
            await context.SaveChangesAsync();
        }
    }
}