using Application.Errors;

namespace Application.Features.Users;

public record GetUsersAvailableForProjectQuery(Guid OrganizationId, Guid ProjectId) : IRequest<Result<UsersSearchVM>>;

internal class GetUsersAvailableForProjectQueryValidator : AbstractValidator<GetUsersAvailableForProjectQuery>
{
    public GetUsersAvailableForProjectQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetUsersAvailableForProjectHandler : IRequestHandler<GetUsersAvailableForProjectQuery, Result<UsersSearchVM>>
{
    private readonly AppDbContext _dbContext;

    public GetUsersAvailableForProjectHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UsersSearchVM>> Handle(GetUsersAvailableForProjectQuery request, CancellationToken cancellationToken)
    {
        var projectMembersUserIds = await _dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.Id == request.ProjectId)
            .Select(x => x.Members.Select(xx => xx.UserId))
            .FirstOrDefaultAsync();
        if (projectMembersUserIds is null)
        {
            return Result.Fail<UsersSearchVM>(new ApplicationError("Project with this ID does not exist"));
        }

        var organizationMembersUserIds = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == request.OrganizationId)
            .Select(x => x.Members.Select(xx => xx.UserId))
            .FirstOrDefaultAsync();
        if(organizationMembersUserIds is null)
        {
            return Result.Fail<UsersSearchVM>(new ApplicationError("Organization with this ID does not exist"));
        }

        var usersIds = organizationMembersUserIds.Except(projectMembersUserIds)
            .ToHashSet();

        var users = await _dbContext.Users
            .Where(x => usersIds.Contains(x.Id))
            .Select(x => new UserSearchVM
            {
                Id = x.Id,
                Name = x.Email,
            })
            .ToListAsync();

        return new UsersSearchVM(users);
    }
}
