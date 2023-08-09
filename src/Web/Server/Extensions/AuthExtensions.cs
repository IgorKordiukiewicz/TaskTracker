using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
            options.AddPolicy("OrganizationMember", policy => policy.Requirements.Add(new OrganizationMemberRequirement())); // TODO: Check if policy.RequireAuthenticatedUser() should be added?
            options.AddPolicy("ProjectMember", policy => policy.Requirements.Add(new ProjectMemberRequirement()));
        });
        services.AddScoped<IAuthorizationHandler, OrganizationMemberRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberRequirementHandler>();

        return services;
    }
}
