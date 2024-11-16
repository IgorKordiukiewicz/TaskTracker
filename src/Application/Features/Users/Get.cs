using Domain.Users;

namespace Application.Features.Users;

public record GetUserQuery(Guid Id) : IRequest<Result<UserVM>>;

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
            .Where(x => x.Id == request.Id)
            .SingleAsync();

        return Result.Ok(new UserVM
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email
        });
    }
}
