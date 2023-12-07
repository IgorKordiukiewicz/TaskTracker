using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Server.RequirementHandlers;

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

        services.AddAuthorization(options => options.AddPolicies());
        services.AddScoped<IAuthorizationHandler, OrganizationMemberRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberRequirementHandler>();

        return services;
    }
}
