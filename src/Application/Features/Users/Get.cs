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

        if(user is null)
        {
            return Result.Fail<UserVM>(new ApplicationError("User with this AuthID does not exist."));
        }

        var organizationsMember = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == user.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var projectsMember = await _dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == user.Id))
            .Select(x => x.Id)
            .ToListAsync();

        return Result.Ok(new UserVM(user.Id, user.FullName, user.Email, organizationsMember, projectsMember));
    }
}
