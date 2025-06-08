namespace Application.Features.Projects;

public record GetProjectInvitationsForUserQuery(Guid UserId) : IRequest<Result<UserProjectInvitationsVM>>;

internal class GetProjectInvitationsForUserQueryValidator : AbstractValidator<GetProjectInvitationsForUserQuery>
{
    public GetProjectInvitationsForUserQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

internal class GetProjectInvitationsForUserHandler(AppDbContext dbContext)
    : IRequestHandler<GetProjectInvitationsForUserQuery, Result<UserProjectInvitationsVM>>
{
    public async Task<Result<UserProjectInvitationsVM>> Handle(GetProjectInvitationsForUserQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstAsync(x => x.Id == request.UserId, cancellationToken);

        var invitations = await dbContext.ProjectInvitations
            .Where(x => x.UserId == user.Id && x.State == ProjectInvitationState.Pending)
            .Join(dbContext.Projects,
            invitation => invitation.ProjectId,
            project => project.Id,
            (invitation, project) => new UserProjectInvitationVM(invitation.Id, project.Id, project.Name))
            .ToListAsync(cancellationToken);

        return Result.Ok(new UserProjectInvitationsVM(invitations));
    }
}