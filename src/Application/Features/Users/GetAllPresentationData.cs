using Domain.Users;

namespace Application.Features.Users;

public record GetAllUsersPresentationDataQuery(Guid UserId) : IRequest<Result<UsersPresentationDataVM>>;


internal class GetAllUsersPresentationDataHandler(AppDbContext dbContext) 
    : IRequestHandler<GetAllUsersPresentationDataQuery, Result<UsersPresentationDataVM>>
{
    public async Task<Result<UsersPresentationDataVM>> Handle(GetAllUsersPresentationDataQuery request, CancellationToken cancellationToken)
    {
        // Only return users that belong to the organizations that the given user also belongs to, to optimize it
        var possibleUsersIds = await dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == request.UserId))
            .SelectMany(x => x.Members.Select(xx => xx.UserId))
            .Distinct()
            .ToListAsync(cancellationToken);

        // Ensure at least the current user is included
        if(possibleUsersIds.Count == 0)
        {
            possibleUsersIds.Add(request.UserId);
        }

        var presentationData = await dbContext.UsersPresentationData
            .Where(x => possibleUsersIds.Contains(x.UserId))
            .Join(dbContext.Users,
            presentationData => presentationData.UserId,
            user => user.Id,
            (presentationData, user) => new UserPresentationDataVM(user.Id, user.FirstName, user.LastName, presentationData.AvatarColor))
            .ToListAsync(cancellationToken);

        return Result.Ok(new UsersPresentationDataVM(presentationData));
    }
}
