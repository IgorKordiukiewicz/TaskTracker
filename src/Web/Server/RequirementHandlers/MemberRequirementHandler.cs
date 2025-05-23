using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.RequirementHandlers;

public class ProjectMemberRequirement : IAuthorizationRequirement
{
    public ProjectPermissions? Permissions { get; init; }
    public bool Owner { get; init; }

    public ProjectMemberRequirement(ProjectPermissions? permissions = null, bool owner = false)
    {
        Permissions = permissions;
        Owner = owner;
    }
}

public class ProjectMemberRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor) 
    : AuthorizationHandler<ProjectMemberRequirement>
{
    private readonly string _idKey = "projectId";

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectMemberRequirement requirement)
    {
        if(!await CheckRequirement(context, requirement))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private async Task<bool> CheckRequirement(AuthorizationHandlerContext context, ProjectMemberRequirement requirement)
    {
        var entityId = GetEntityId(contextAccessor.HttpContext);
        if (entityId == default)
        {
            return false;
        }

        var userId = context.User.GetUserId();
        if (userId == default)
        {
            return false;
        }

        var member = await dbContext.Projects
            .AsNoTracking()
            .Include(x => x.Members)
            .Where(x => x.Id == entityId)
            .SelectMany(x => x.Members)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (member is null)
        {
            return false;
        }

        if (requirement.Owner)
        {
            return await dbContext.Projects.AnyAsync(x => x.Id == entityId && x.OwnerId == userId);
        }

        if (requirement.Permissions is null)
        {
            return true;
        }

        var role = await dbContext.ProjectRoles
            .AsNoTracking()
            .Where(x => x.Id == member.RoleId)
            .FirstAsync();
        return role.HasPermission(requirement.Permissions.Value);
    }

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
