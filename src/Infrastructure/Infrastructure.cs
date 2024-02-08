using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.Workflows;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Infrastructure
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<IRepository<Organization>, OrganizationRepository>();
        services.AddScoped<IRepository<Project>, ProjectRepository>();
        services.AddScoped<IRepository<Domain.Tasks.Task>, TaskRepository>();
        services.AddScoped<IRepository<Workflow>, WorkflowRepository>();

        return services;
    }
}
