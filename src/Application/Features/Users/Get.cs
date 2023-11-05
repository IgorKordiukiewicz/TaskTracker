using Application.Errors;

namespace Application.Features.Users;

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
            .Where(x => x.AuthenticationId == request.AuthenticationId)
            .Select(x => new UserVM(x.Id, x.FullName, x.Email))
            .SingleOrDefaultAsync();

        if(user is null)
        {
            return Result.Fail<UserVM>(new ApplicationError("User with this AuthID does not exist."));
        }

        return user;
    }
}
