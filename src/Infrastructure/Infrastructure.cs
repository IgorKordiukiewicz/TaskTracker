using Domain.Notifications;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Domain.Workflows;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;

namespace Infrastructure;

public static class Infrastructure
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<IRepository<Project>, ProjectRepository>();
        services.AddScoped<IRepository<Domain.Tasks.Task>, TaskRepository>();
        services.AddScoped<IRepository<Workflow>, WorkflowRepository>();
        services.AddScoped<IRepository<TaskRelationManager>, TaskRelationManagerRepository>();
        services.AddScoped<IRepository<Notification>, NotificationRepository>();

        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(configuration.GetConnectionString("BlobStorageConnection"));
        });

        return services;
    }
}
