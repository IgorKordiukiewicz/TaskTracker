using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Shared;
using Shared.Enums;
using Web.Server.Requirements;

namespace Web.Server.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
        {
            c.Authority = configuration["Auth0:Domain"];
            c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidAudience = configuration["Auth0:Audience"],
                ValidIssuer = configuration["Auth0:Domain"]
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.OrganizationMember, policy => policy.Requirements.Add(new OrganizationMemberRequirement()));
            options.AddPolicy(Policy.OrganizationMembersEditor, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.Members)));
            options.AddPolicy(Policy.OrganizationProjectsEditor, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.Projects)));

            options.AddPolicy(Policy.ProjectMember, policy => policy.Requirements.Add(new ProjectMemberRequirement()));
            options.AddPolicy(Policy.ProjectMembersEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Members)));
            options.AddPolicy(Policy.ProjectTasksEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Tasks)));
            options.AddPolicy(Policy.ProjectWorkflowsEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Workflows)));
        });
        services.AddScoped<IAuthorizationHandler, OrganizationMemberRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberRequirementHandler>();

        return services;
    }
}
