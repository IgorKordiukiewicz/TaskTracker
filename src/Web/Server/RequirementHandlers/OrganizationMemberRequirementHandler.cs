using Domain.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.RequirementHandlers;

public class OrganizationMemberRequirement : MemberRequirement<OrganizationPermissions>
{
    public bool Owner { get; init; }

    public OrganizationMemberRequirement(OrganizationPermissions? permissions = null, bool owner = false)
    {
        Permissions = permissions;
        Owner = owner;
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

        if(requirement.Owner)
        {
            return await dbContext.Organizations
                .AnyAsync(x => x.Id == entityId && x.OwnerId == userId);
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
