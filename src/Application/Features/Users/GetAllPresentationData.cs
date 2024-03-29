﻿using Domain.Users;

namespace Application.Features.Users;

public record GetAllUsersPresentationDataQuery(string AuthenticationId) : IRequest<Result<UsersPresentationDataVM>>;

internal class GetAllUsersPresentationDataQueryValidator : AbstractValidator<GetAllUsersPresentationDataQuery>
{
    public GetAllUsersPresentationDataQueryValidator()
    {
        RuleFor(x => x.AuthenticationId).NotEmpty();
    }
}

internal class GetAllUsersPresentationDataHandler : IRequestHandler<GetAllUsersPresentationDataQuery, Result<UsersPresentationDataVM>>
{
    private readonly AppDbContext _dbContext;

    public GetAllUsersPresentationDataHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UsersPresentationDataVM>> Handle(GetAllUsersPresentationDataQuery request, CancellationToken cancellationToken)
    {
        var userId = await _dbContext.Users
            .Where(x => x.AuthenticationId == request.AuthenticationId)
            .Select(x => x.Id)
            .SingleOrDefaultAsync();

        if(userId == default)
        {
            return Result.Fail<UsersPresentationDataVM>(new NotFoundError<User>($"Authentication ID: {request.AuthenticationId}"));
        }

        // Only return users that belong to the organizations that the given user also belongs to, to optimize it
        var possibleUsersIds = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == userId))
            .SelectMany(x => x.Members.Select(xx => xx.UserId))
            .Distinct()
            .ToListAsync();

        var presentationData = await _dbContext.UsersPresentationData
            .Where(x => possibleUsersIds.Contains(x.UserId))
            .Select(x => new UserPresentationDataVM(x.UserId, x.AvatarColor))
            .ToListAsync();

        return Result.Ok(new UsersPresentationDataVM(presentationData));
    }
}
