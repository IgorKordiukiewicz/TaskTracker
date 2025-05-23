using Domain.Projects;

namespace Application.Features.Users;

public record GetUsersAvailableForProjectQuery(Guid ProjectId) : IRequest<Result<UsersSearchVM>>;

internal class GetUsersAvailableForProjectQueryValidator : AbstractValidator<GetUsersAvailableForProjectQuery>
{
    public GetUsersAvailableForProjectQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetUsersAvailableForProjectHandler(AppDbContext dbContext) 
    : IRequestHandler<GetUsersAvailableForProjectQuery, Result<UsersSearchVM>>
{
    public async Task<Result<UsersSearchVM>> Handle(GetUsersAvailableForProjectQuery request, CancellationToken cancellationToken)
    {
        var projectMembersUserIds = await dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.Id == request.ProjectId)
            .Select(x => x.Members.Select(xx => xx.UserId))
            .FirstOrDefaultAsync(cancellationToken);
        if (projectMembersUserIds is null)
        {
            return Result.Fail<UsersSearchVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var organizationId = await dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Select(x => x.OrganizationId)
            .FirstAsync(cancellationToken);

        var organizationMembersUserIds = new List<Guid>(); // TODO

        var usersIds = organizationMembersUserIds.Except(projectMembersUserIds)
            .ToHashSet();

        var users = await dbContext.Users
            .Where(x => usersIds.Contains(x.Id))
            .Select(x => new UserSearchVM
            {
                Id = x.Id,
                Email = x.Email,
            })
            .ToListAsync(cancellationToken);

        return new UsersSearchVM(users);
    }
}
