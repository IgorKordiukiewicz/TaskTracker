using Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.Requirements;

public class ProjectMemberRequirement : IAuthorizationRequirement
{
}

public class ProjectMemberRequirementHandler : MemberRequirementHandler<ProjectMemberRequirement>
{
    public ProjectMemberRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
        : base(dbContext, contextAccessor, "projectId")
    {
    }

    protected override async Task<bool> CheckRequirement(AppDbContext dbContext, Guid userId, Guid entityId)
        => await dbContext.Projects.Include(x => x.Members)
        .AnyAsync(x => x.Id == entityId && x.Members.Any(xx => xx.UserId == userId));
}