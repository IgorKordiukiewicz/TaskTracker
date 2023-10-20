using Application.Errors;
using Domain.Organizations;
using Shared.Enums;

namespace Application.Features.Organizations;

public record GetOrganizationInvitationsForUserQuery(string UserAuthenticationId) : IRequest<Result<UserOrganizationInvitations>>;

internal class GetOrganizationInvitationsForUserQueryValidator : AbstractValidator<GetOrganizationInvitationsForUserQuery>
{
    public GetOrganizationInvitationsForUserQueryValidator()
    {
        RuleFor(x => x.UserAuthenticationId).NotEmpty();
    }
}

internal class GetOrganizationInvitationsForUserHandler : IRequestHandler<GetOrganizationInvitationsForUserQuery, Result<UserOrganizationInvitations>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationInvitationsForUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserOrganizationInvitations>> Handle(GetOrganizationInvitationsForUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AuthenticationId == request.UserAuthenticationId);
        if(user is null)
        {
            return Result.Fail<UserOrganizationInvitations>(new ApplicationError("User with this authentication ID does not exist."));
        }

        var invitations = await _dbContext.OrganizationInvitations
            .Where(x => x.UserId == user.Id && x.State == OrganizationInvitationState.Pending)
            .Join(_dbContext.Organizations,
            invitation => invitation.OrganizationId,
            organization => organization.Id,
            (invitation, organization) => new UserOrganizationInvitationVM(invitation.Id, organization.Name))
            .ToListAsync();

        return Result.Ok(new UserOrganizationInvitations(invitations));
    }
}
