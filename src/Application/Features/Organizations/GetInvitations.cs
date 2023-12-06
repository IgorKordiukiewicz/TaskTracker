using Application.Errors;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationInvitationsQuery(Guid OrganizationId) : IRequest<Result<OrganizationInvitationsVM>>;

internal class GetOrganizationInvitationsQueryValidator : AbstractValidator<GetOrganizationInvitationsQuery>
{
    public GetOrganizationInvitationsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationInvitationsHandler : IRequestHandler<GetOrganizationInvitationsQuery, Result<OrganizationInvitationsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationInvitationsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationInvitationsVM>> Handle(GetOrganizationInvitationsQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<OrganizationInvitationsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var invitations = await _dbContext.OrganizationInvitations.Where(x => x.OrganizationId == request.OrganizationId)
            .Join(_dbContext.Users,
            invitation => invitation.UserId,
            user => user.Id,
            (invitation, user) => new OrganizationInvitationVM(invitation.Id, user.Email, invitation.State))
            .ToListAsync();

        return Result.Ok(new OrganizationInvitationsVM(invitations));
    }
}
