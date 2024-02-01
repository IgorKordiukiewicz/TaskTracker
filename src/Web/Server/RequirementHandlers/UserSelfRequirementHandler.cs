using Application.Data;
using Microsoft.EntityFrameworkCore;

namespace Web.Server.RequirementHandlers;

public class UserSelfRequirementHandler : AuthorizationHandler<UserSelfRequirement>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserSelfRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserSelfRequirement requirement)
    {
        var routeUserId = _contextAccessor.HttpContext?.Request.RouteValues["userId"]?.ToString() ?? string.Empty;
        if(!Guid.TryParse(routeUserId, out var userId))
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

        var result = await _dbContext.Users.AnyAsync(x => x.Id == userId && x.AuthenticationId == userAuthId);
        if(result)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
