using Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.Requirements;

public class OrganizationMemberRequirement : IAuthorizationRequirement
{
}

public class OrganizationMemberHandler : AuthorizationHandler<OrganizationMemberRequirement>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public OrganizationMemberHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    // TODO: Add caching to improve performance? 
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationMemberRequirement requirement)
    {
        if (!Guid.TryParse(_contextAccessor.HttpContext?.Request.Headers["OrganizationId"].ToString(), out var organizationId))
        {
            context.Fail();
            return;
        }

        var userAuthId = context.User.GetUserAuthenticationId();
        if(string.IsNullOrWhiteSpace(userAuthId))
        {
            context.Fail();
            return;
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.AuthenticationId == userAuthId);
        if(user is null)
        {
            context.Fail();
            return;
        }

        if(!await _dbContext.Organizations.Include(x => x.Members)
            .AnyAsync(x => x.Id == organizationId && x.Members.Any(xx => xx.UserId == user.Id)))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);

        return;
    }
}
