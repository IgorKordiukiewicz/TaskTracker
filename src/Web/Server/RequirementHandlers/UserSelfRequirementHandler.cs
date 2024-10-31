using Infrastructure;
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

        var currentUserId = context.User.GetUserId(); // TODO: refactor, is this whole req handler even necessary?
        if (currentUserId != userId)
        {
            context.Fail();
            return;
        }

        var result = await _dbContext.Users.AnyAsync(x => x.Id == userId);
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
