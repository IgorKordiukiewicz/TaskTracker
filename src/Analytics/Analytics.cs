using Analytics.Infrastructure;
using Analytics.ProjectionHandlers;
using Analytics.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics;

public static class Analytics
{
    public static IServiceCollection AddAnalyticsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AnalyticsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("AnalyticsConnection")));

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IProjectionRebuilder, ProjectionRebuilder>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IQueryService, QueryService>();

        services.AddScoped<IProjectionHandler, DailyTotalTaskStatusHandler>();
        services.AddScoped<IProjectionHandler, DailyTotalTaskPriorityHandler>();
        services.AddScoped<IProjectionHandler, DailyTotalTaskAssigneeHandler>();

        return services;
    }
}
