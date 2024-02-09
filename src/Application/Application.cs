using System.Reflection;
using Application.Behaviors;
using Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Application
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        services.AddValidatorsFromAssembly(assembly);

        // Order matters; error logging has to be registered before validation,
        // otherwise error logging will not get called if validation returns failure
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));  

        services.AddScoped<IJobsService, JobsService>();

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
