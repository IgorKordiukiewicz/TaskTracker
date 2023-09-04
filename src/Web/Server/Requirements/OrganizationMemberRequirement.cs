using Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.Requirements;

public class OrganizationMemberRequirement : IAuthorizationRequirement
{
}

public class OrganizationMemberRequirementHandler : MemberRequirementHandler<OrganizationMemberRequirement>
{
    public OrganizationMemberRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
        : base(dbContext, contextAccessor, "organizationId")
    {
    }

    protected override async Task<bool> CheckRequirement(AppDbContext dbContext, Guid userId, Guid entityId)
        => await dbContext.Organizations.Include(x => x.Members)
        .AnyAsync(x => x.Id == entityId && x.Members.Any(xx => xx.UserId == userId));
}
