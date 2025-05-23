using Domain.Projects;

namespace Application.Features.Users;

public record GetUsersAvailableForProjectInvitationQuery(Guid ProjectId, string SearchValue) : IRequest<Result<UsersSearchVM>>;

internal class GetUsersAvailableForProjectInvitationQueryValidator : AbstractValidator<GetUsersAvailableForProjectInvitationQuery>
{
    public GetUsersAvailableForProjectInvitationQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.SearchValue).NotEmpty();
    }
}

internal class GetUsersAvailableForProjectInvitationHandler(AppDbContext dbContext)
    : IRequestHandler<GetUsersAvailableForProjectInvitationQuery, Result<UsersSearchVM>>
{
    public async Task<Result<UsersSearchVM>> Handle(GetUsersAvailableForProjectInvitationQuery request, CancellationToken cancellationToken)
    {
        // TODO: Possible performance bottleneck?
        // Create a view which has a list of (UserId, UserName, ProjectId) ?
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(x => x.Invitations)
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Fail<UsersSearchVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var membersUsers = project.Members.Select(x => x.UserId);
        var usersWithPendingInvitations = project.Invitations
            .Where(x => x.State == ProjectInvitationState.Pending)
            .Select(x => x.UserId)
            .Distinct();
        var unavailableUsers = membersUsers.Concat(usersWithPendingInvitations);

        var searchValue = request.SearchValue.ToLower();
        var users = await dbContext.Users
            .Where(x => !unavailableUsers.Contains(x.Id) && x.Email.ToLower().Contains(searchValue))
            .Take(5)
            .Select(x => new UserSearchVM
            {
                Id = x.Id,
                Email = x.Email,
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(new UsersSearchVM(users));
    }
}