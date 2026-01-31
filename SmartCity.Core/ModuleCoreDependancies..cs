using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartCity.AppCore.Behaviors;
using System.Reflection;

namespace SmartCity.AppCore
{
    public static class ModuleCoreDependancies
    {
        public static IServiceCollection AddCoreDependancies(this IServiceCollection services)
        {
            // Get Validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // 
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ModuleCoreDependancies).Assembly));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
