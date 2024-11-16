using Domain.Users;

namespace Application.Features.Users;

public record GetAllUsersPresentationDataQuery(Guid UserId) : IRequest<Result<UsersPresentationDataVM>>;


internal class GetAllUsersPresentationDataHandler : IRequestHandler<GetAllUsersPresentationDataQuery, Result<UsersPresentationDataVM>>
{
    private readonly AppDbContext _dbContext;

    public GetAllUsersPresentationDataHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UsersPresentationDataVM>> Handle(GetAllUsersPresentationDataQuery request, CancellationToken cancellationToken)
    {
        // Only return users that belong to the organizations that the given user also belongs to, to optimize it
        var possibleUsersIds = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == request.UserId))
            .SelectMany(x => x.Members.Select(xx => xx.UserId))
            .Distinct()
            .ToListAsync();

        var presentationData = await _dbContext.UsersPresentationData
            .Where(x => possibleUsersIds.Contains(x.UserId))
            .Join(_dbContext.Users,
            presentationData => presentationData.UserId,
            user => user.Id,
            (presentationData, user) => new UserPresentationDataVM(user.Id, user.FirstName, user.LastName, presentationData.AvatarColor))
            .ToListAsync();

        return Result.Ok(new UsersPresentationDataVM(presentationData));
    }
}
