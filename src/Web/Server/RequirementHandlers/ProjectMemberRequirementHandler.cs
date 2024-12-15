using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.RequirementHandlers;

public class ProjectMemberRequirement : MemberRequirement<ProjectPermissions>
{
    public ProjectMemberRequirement(ProjectPermissions? permissions = null)
    {
        Permissions = permissions;
    }
}

public class ProjectMemberRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor) 
    : MemberRequirementHandler<ProjectMemberRequirement>(dbContext, contextAccessor, "projectId")
{
    protected override async Task<bool> CheckRequirement(ProjectMemberRequirement requirement, AppDbContext dbContext, Guid userId, Guid entityId)
    {
        var member = await dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.Id == entityId)
            .SelectMany(x => x.Members)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (member is null)
        {
            return false;
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
}