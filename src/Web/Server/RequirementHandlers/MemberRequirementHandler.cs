﻿using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.RequirementHandlers;

public abstract class MemberRequirementHandler<TAuthorizationRequirement> : AuthorizationHandler<TAuthorizationRequirement>
    where TAuthorizationRequirement : IAuthorizationRequirement
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly string _idKey;

    protected MemberRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor, string idKey)
    {
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
        _idKey = idKey;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TAuthorizationRequirement requirement)
    {
        var entityId = GetEntityId(_contextAccessor.HttpContext);
        if (entityId == default)
        {
            context.Fail();
            return;
        }

        var userAuthId = context.User.GetUserAuthenticationId();
        if (string.IsNullOrWhiteSpace(userAuthId))
        {
            context.Fail();
            return;
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.AuthenticationId == userAuthId);
        if (user is null)
        {
            context.Fail();
            return;
        }

        if(!await CheckRequirement(requirement, _dbContext, user.Id, entityId))
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

        var routeValue = httpContext.Request.RouteValues[_idKey]?.ToString() ?? string.Empty;
        if (Guid.TryParse(routeValue, out var routeId))
        {
            return routeId;
        }

        var headerValue = httpContext.Request.Headers[_idKey].ToString() ?? string.Empty;
        if (Guid.TryParse(headerValue, out var headerId))
        {
            return headerId;
        }

        var paramValue = httpContext.Request.Query[_idKey].FirstOrDefault() ?? string.Empty;
        if (Guid.TryParse(paramValue, out var paramId))
        {
            return paramId;
        }

        return default;
    }
}
