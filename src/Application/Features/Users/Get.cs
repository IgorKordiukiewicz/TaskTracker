using Application.Errors;

namespace Application.Features.Users;

// TODO: Rename to GetUserAuthData or GetUserMembershipData or sth
public record GetUserQuery(string AuthenticationId) : IRequest<Result<UserVM>>;

internal class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(x => x.AuthenticationId).NotEmpty();
    }
}

internal class GetUserHandler : IRequestHandler<GetUserQuery, Result<UserVM>>
{
    private readonly AppDbContext _dbContext;

    public GetUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserVM>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.AuthenticationId == request.AuthenticationId)
            .SingleOrDefaultAsync();

        if (user is null)
        {
            return Result.Fail<UserVM>(new ApplicationError("User with this AuthID does not exist."));
        }

        var permissionsByOrganization = await _dbContext.Organizations
            .Include(x => x.Members)
            .Include(x => x.Roles)
            .Where(x => x.Members.Any(xx => xx.UserId == user.Id))
            .Select(x => new { x.Id, x.Roles.First(xx => xx.Id == x.Members.First(xxx => xxx.UserId == user.Id).RoleId).Permissions })
            .ToDictionaryAsync(k => k.Id, v => v.Permissions);

        var permissionsByProject = await _dbContext.Projects
            .Include(x => x.Members)
            .Include(x => x.Roles)
            .Where(x => x.Members.Any(xx => xx.UserId == user.Id))
            .Select(x => new { x.Id, x.Roles.First(xx => xx.Id == x.Members.First(xxx => xxx.UserId == user.Id).RoleId).Permissions })
            .ToDictionaryAsync(k => k.Id, v => v.Permissions);

        return Result.Ok(new UserVM
        {
            Id = user.Id,
            Name = user.FullName,
            Email = user.Email,
            PermissionsByOrganization = permissionsByOrganization,
            PermissionsByProject = permissionsByProject
        });
    }
}
