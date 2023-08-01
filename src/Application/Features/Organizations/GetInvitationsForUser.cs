using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationInvitationsForUserQuery(string UserAuthenticationId) : IRequest<Result<OrganizationInvitationsVM>>;

internal class GetOrganizationInvitationsForUserQueryValidator : AbstractValidator<GetOrganizationInvitationsForUserQuery>
{
    public GetOrganizationInvitationsForUserQueryValidator()
    {
        RuleFor(x => x.UserAuthenticationId).NotEmpty();
    }
}

internal class GetOrganizationInvitationsForUserHandler : IRequestHandler<GetOrganizationInvitationsForUserQuery, Result<OrganizationInvitationsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationInvitationsForUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationInvitationsVM>> Handle(GetOrganizationInvitationsForUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.AuthenticationId == request.UserAuthenticationId);
        if(user is null)
        {
            return Result.Fail<OrganizationInvitationsVM>(new Error("User with this authentication ID does not exist."));
        }

        var invitations = await _dbContext.OrganizationInvitations
            .Where(x => x.UserId == user.Id && x.State == OrganizationInvitationState.Pending)
            .Join(_dbContext.Organizations,
            invitation => invitation.OrganizationId,
            organization => organization.Id,
            (invitation, organization) => new OrganizationInvitationVM(invitation.Id, organization.Name))
            .ToListAsync();

        return Result.Ok(new OrganizationInvitationsVM(invitations));
    }
}
