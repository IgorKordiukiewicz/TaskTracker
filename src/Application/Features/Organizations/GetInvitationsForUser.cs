using Domain.Users;
using Shared.Enums;

namespace Application.Features.Organizations;

public record GetOrganizationInvitationsForUserQuery(string UserAuthenticationId) : IRequest<Result<UserOrganizationInvitationsVM>>;

internal class GetOrganizationInvitationsForUserQueryValidator : AbstractValidator<GetOrganizationInvitationsForUserQuery>
{
    public GetOrganizationInvitationsForUserQueryValidator()
    {
        RuleFor(x => x.UserAuthenticationId).NotEmpty();
    }
}

internal class GetOrganizationInvitationsForUserHandler : IRequestHandler<GetOrganizationInvitationsForUserQuery, Result<UserOrganizationInvitationsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationInvitationsForUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserOrganizationInvitationsVM>> Handle(GetOrganizationInvitationsForUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AuthenticationId == request.UserAuthenticationId);
        if(user is null)
        {
            return Result.Fail<UserOrganizationInvitationsVM>(new NotFoundError<User>($"AuthID: {request.UserAuthenticationId}"));
        }

        var invitations = await _dbContext.OrganizationInvitations
            .Where(x => x.UserId == user.Id && x.State == OrganizationInvitationState.Pending)
            .Join(_dbContext.Organizations,
            invitation => invitation.OrganizationId,
            organization => organization.Id,
            (invitation, organization) => new UserOrganizationInvitationVM(invitation.Id, organization.Name))
            .ToListAsync();

        return Result.Ok(new UserOrganizationInvitationsVM(invitations));
    }
}
