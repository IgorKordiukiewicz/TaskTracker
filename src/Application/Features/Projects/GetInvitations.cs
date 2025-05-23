using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectInvitationsQuery(Guid ProjectId) : IRequest<Result<ProjectInvitationsVM>>;

internal class GetProjectInvitationsQueryValidator : AbstractValidator<GetProjectInvitationsQuery>
{
    public GetProjectInvitationsQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectInvitationsHandler(AppDbContext dbContext)
    : IRequestHandler<GetProjectInvitationsQuery, Result<ProjectInvitationsVM>>
{
    public async Task<Result<ProjectInvitationsVM>> Handle(GetProjectInvitationsQuery request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<ProjectInvitationsVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var query = dbContext.ProjectInvitations
            .AsNoTracking()
            .Where(x => x.ProjectId == request.ProjectId)
            .Join(dbContext.Users,
            invitation => invitation.UserId,
            user => user.Id,
            (invitation, user) => new { Invitation = invitation, User = user });

        var invitations = await query
            .OrderByDescending(x => x.Invitation.CreatedAt)
            .Select(x => new ProjectInvitationVM(x.Invitation.Id, x.User.Email, x.Invitation.State, x.Invitation.CreatedAt, x.Invitation.FinalizedAt, x.Invitation.ExpirationDate))
            .ToListAsync(cancellationToken);

        return Result.Ok(new ProjectInvitationsVM(invitations));
    }
}