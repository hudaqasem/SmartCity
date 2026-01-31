using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartCity.Domain.Helper;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Data;
using System.Security.Claims;
using System.Text;


namespace SmartCity.Infrastructure
{
    public static class ServiceRegiteration
    {
        public static IServiceCollection AddServiceRegiteration(this IServiceCollection services, IConfiguration configuration)
        {
            // Identity Configuration
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;

                options.User.RequireUniqueEmail = true;
                // options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //JWT Authentication
            var jwtSettings = new JwtSettings();
            configuration.GetSection(nameof(jwtSettings)).Bind(jwtSettings);

            services.AddSingleton(jwtSettings);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(x =>
          {
              x.RequireHttpsMetadata = false;
              x.SaveToken = true;
              x.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = jwtSettings.ValidateIssuer,
                  ValidIssuer = jwtSettings.Issuer,
                  ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                  ValidAudience = jwtSettings.Audience,
                  ValidateAudience = jwtSettings.ValidateAudience,
                  ValidateLifetime = jwtSettings.ValidateLifeTime,
                  RoleClaimType = ClaimTypes.Role,
                  ClockSkew = TimeSpan.FromMinutes(1)
              };
          });




            return services;
        }
    }
}
