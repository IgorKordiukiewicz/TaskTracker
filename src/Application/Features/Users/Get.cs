using Domain.Users;

namespace Application.Features.Users;

public record GetUserQuery(Guid Id) : IRequest<Result<UserVM>>;

internal class GetUserHandler(AppDbContext dbContext) 
    : IRequestHandler<GetUserQuery, Result<UserVM>>
{
    public async Task<Result<UserVM>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .SingleAsync(cancellationToken);

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
