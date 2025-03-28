﻿using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationInvitationsQuery(Guid OrganizationId) : IRequest<Result<OrganizationInvitationsVM>>;

internal class GetOrganizationInvitationsQueryValidator : AbstractValidator<GetOrganizationInvitationsQuery>
{
    public GetOrganizationInvitationsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationInvitationsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationInvitationsQuery, Result<OrganizationInvitationsVM>>
{
    public async Task<Result<OrganizationInvitationsVM>> Handle(GetOrganizationInvitationsQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId, cancellationToken))
        {
            return Result.Fail<OrganizationInvitationsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var query = dbContext.OrganizationInvitations
            .AsNoTracking()
            .Where(x => x.OrganizationId == request.OrganizationId)
            .Join(dbContext.Users,
            invitation => invitation.UserId,
            user => user.Id,
            (invitation, user) => new { Invitation = invitation, User = user });

        var invitations = await query
            .OrderByDescending(x => x.Invitation.CreatedAt)
            .Select(x => new OrganizationInvitationVM(x.Invitation.Id, x.User.Email, x.Invitation.State, x.Invitation.CreatedAt, x.Invitation.FinalizedAt, x.Invitation.ExpirationDate))
            .ToListAsync(cancellationToken);

        return Result.Ok(new OrganizationInvitationsVM(invitations));
    }
}
