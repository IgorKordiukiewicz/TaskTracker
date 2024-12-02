using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.RequirementHandlers;

public abstract class MemberRequirement<TPermissions> : IAuthorizationRequirement
    where TPermissions : struct, Enum
{
    public TPermissions? Permissions { get; init; }
}

public abstract class MemberRequirementHandler<TAuthorizationRequirement>(AppDbContext dbContext, IHttpContextAccessor contextAccessor, string idKey) 
    : AuthorizationHandler<TAuthorizationRequirement>
    where TAuthorizationRequirement : IAuthorizationRequirement
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TAuthorizationRequirement requirement)
    {
        var entityId = GetEntityId(contextAccessor.HttpContext);
        if (entityId == default)
        {
            context.Fail();
            return;
        }

        var userId = context.User.GetUserId();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            context.Fail();
            return;
        }

        if(!await CheckRequirement(requirement, dbContext, user.Id, entityId))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
        return;
    }

    protected abstract Task<bool> CheckRequirement(TAuthorizationRequirement requirement, AppDbContext dbContext, Guid userId, Guid entityId);

    private Guid GetEntityId(HttpContext? httpContext)
    {
        if (httpContext is null)
        {
            return default;
        }

        var routeValue = httpContext.Request.RouteValues[idKey]?.ToString() ?? string.Empty;
        if (Guid.TryParse(routeValue, out var routeId))
        {
            return routeId;
        }

        var headerValue = httpContext.Request.Headers[idKey].ToString() ?? string.Empty;
        if (Guid.TryParse(headerValue, out var headerId))
        {
            return headerId;
        }

        var paramValue = httpContext.Request.Query[idKey].FirstOrDefault() ?? string.Empty;
        if (Guid.TryParse(paramValue, out var paramId))
        {
            return paramId;
        }

        return default;
    }
}
