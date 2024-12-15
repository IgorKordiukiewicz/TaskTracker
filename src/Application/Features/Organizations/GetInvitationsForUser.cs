namespace Application.Features.Organizations;

public record GetOrganizationInvitationsForUserQuery(Guid UserId) : IRequest<Result<UserOrganizationInvitationsVM>>;

internal class GetOrganizationInvitationsForUserQueryValidator : AbstractValidator<GetOrganizationInvitationsForUserQuery>
{
    public GetOrganizationInvitationsForUserQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

internal class GetOrganizationInvitationsForUserHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationInvitationsForUserQuery, Result<UserOrganizationInvitationsVM>>
{
    public async Task<Result<UserOrganizationInvitationsVM>> Handle(GetOrganizationInvitationsForUserQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstAsync(x => x.Id == request.UserId, cancellationToken);

        var invitations = await dbContext.OrganizationInvitations
            .Where(x => x.UserId == user.Id && x.State == OrganizationInvitationState.Pending)
            .Join(dbContext.Organizations,
            invitation => invitation.OrganizationId,
            organization => organization.Id,
            (invitation, organization) => new UserOrganizationInvitationVM(invitation.Id, organization.Name))
            .ToListAsync(cancellationToken);

        return Result.Ok(new UserOrganizationInvitationsVM(invitations));
    }
}
