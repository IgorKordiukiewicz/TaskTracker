using Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Web.Server.Requirements;

public class OrganizationMemberRequirement : IAuthorizationRequirement
{
    public OrganizationPermissions? Permissions { get; set; }

    public OrganizationMemberRequirement(OrganizationPermissions? permissions = null)
    {
        Permissions = permissions;
    }
}

public class OrganizationMemberRequirementHandler : MemberRequirementHandler<OrganizationMemberRequirement>
{
    public OrganizationMemberRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
        : base(dbContext, contextAccessor, "organizationId")
    {
    }

    protected override async Task<bool> CheckRequirement(OrganizationMemberRequirement requirement, AppDbContext dbContext, Guid userId, Guid entityId)
    {
        var member = await dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == entityId)
            .SelectMany(x => x.Members)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if(member is null)
        {
            return false;
        }

        if(requirement.Permissions is null)
        {
            return true;
        }

        var role = await dbContext.OrganizationRoles
            .AsNoTracking()
            .Where(x => x.Id == member.RoleId)
            .FirstAsync();
        return role.HasPermission(requirement.Permissions.Value);
    }
}
