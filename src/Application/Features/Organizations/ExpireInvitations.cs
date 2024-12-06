
using Application.Common;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record ExpireOrganizationsInvitationsCommand() : IRequest;

internal class ExpireOrganizationsInvitationsHandler(AppDbContext dbContext, IRepository<Organization> organizationRepository, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<ExpireOrganizationsInvitationsCommand>
{
    public async Task Handle(ExpireOrganizationsInvitationsCommand request, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.Now();
        var organizationsIds = await dbContext.OrganizationInvitations
            .Where(x => x.ExpirationDate.HasValue && x.ExpirationDate.Value < now)
            .Select(x => x.OrganizationId)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var organizationId in organizationsIds)
        {
            var organization = (await organizationRepository.GetById(organizationId, cancellationToken))!;
            organization.ExpireInvitations(now);
            await organizationRepository.Update(organization, cancellationToken);
        }
    }
}
