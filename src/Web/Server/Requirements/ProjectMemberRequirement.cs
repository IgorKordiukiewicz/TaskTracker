using Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Web.Server.Extensions;

namespace Web.Server.Requirements;

public class ProjectMemberRequirement : IAuthorizationRequirement
{
}

// TODO: Add base class/common component for ProjectMemberRequirement & OrganizationMemberRequirement
public class ProjectMemberRequirementHandler : AuthorizationHandler<ProjectMemberRequirement>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public ProjectMemberRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectMemberRequirement requirement)
    {
        var projectId = GetProjectId(_contextAccessor.HttpContext);
        if(projectId == default)
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

        if(!await _dbContext.Projects.Include(x => x.Members)
            .AnyAsync(x => x.Id == projectId && x.Members.Any(xx => xx.UserId == user.Id)))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
        return;
    }

    private static Guid GetProjectId(HttpContext? httpContext)
    {
        if(httpContext is null)
        {
            return default;
        }

        var routeValue = httpContext.Request.RouteValues["projectId"]?.ToString() ?? string.Empty;
        if(Guid.TryParse(routeValue, out var routeId))
        {
            return routeId;
        }

        var headerValue = httpContext.Request.Headers["ProjectId"].ToString() ?? string.Empty;
        if(Guid.TryParse(headerValue, out var headerId))
        {
            return headerId;
        }

        return default;
    }
}