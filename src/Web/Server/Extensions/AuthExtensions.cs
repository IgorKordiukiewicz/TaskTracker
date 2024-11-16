using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Server.RequirementHandlers;

namespace Web.Server.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtSecret"])),
                    ValidAudience = configuration["Authentication:Audience"],
                    ValidIssuer = configuration["Authentication:Issuer"]
                };
            });

        services.AddAuthorization(options => options.AddPolicies());
        services.AddScoped<IAuthorizationHandler, OrganizationMemberRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberRequirementHandler>();

        return services;
    }
}
