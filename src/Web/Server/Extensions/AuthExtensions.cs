using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Web.Server.Requirements;

namespace Web.Server.Extensions;

public static class Policy
{
    public const string OrganizationMember = "OrganizationMember";
    public const string ProjectMember = "ProjectMember";
}

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
            options.AddPolicy(Policy.OrganizationMember, policy => policy.Requirements.Add(new OrganizationMemberRequirement())); // TODO: Check if policy.RequireAuthenticatedUser() should be added?
            options.AddPolicy(Policy.ProjectMember, policy => policy.Requirements.Add(new ProjectMemberRequirement()));
        });
        services.AddScoped<IAuthorizationHandler, OrganizationMemberRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberRequirementHandler>();

        return services;
    }
}
