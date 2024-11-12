using Domain.Organizations;
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
            return Result.Fail<UsersSearchVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var organizationId = await _dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Select(x => x.OrganizationId)
            .FirstAsync();

        var organizationMembersUserIds = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == organizationId)
            .Select(x => x.Members.Select(xx => xx.UserId))
            .FirstAsync();

        var usersIds = organizationMembersUserIds.Except(projectMembersUserIds)
            .ToHashSet();

        var users = await _dbContext.Users
            .Where(x => usersIds.Contains(x.Id))
            .Select(x => new UserSearchVM
            {
                Id = x.Id,
                Email = x.Email,
            })
            .ToListAsync();

        return new UsersSearchVM(users);
    }
}
