using Application.Features.Projects;
using Domain.Organizations;
using Microsoft.Extensions.Logging;

namespace Application.Common;

public interface IJobsService
{
    Task RemoveUserFromOrganizationProjects(Guid userId, Guid organizationId, CancellationToken cancellationToken = default);
    Task ExpireOrganizationsInvitations(CancellationToken cancellationToken = default);
}

public class JobsService(AppDbContext dbContext, IMediator mediator, ILogger<JobsService> logger, 
    IDateTimeProvider dateTimeProvider, IRepository<Organization> organizationRepository) 
    : IJobsService
{
    public async Task RemoveUserFromOrganizationProjects(Guid userId, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var projectsAndMembers = await dbContext.Projects
            .Where(x => x.OrganizationId == organizationId && x.Members.Any(xx => xx.UserId == userId))
            .Select(v => new { ProjectId = v.Id, MemberId = v.Members.First(x => x.UserId == userId).Id })
            .ToListAsync(cancellationToken);

        foreach (var projectAndMember in projectsAndMembers)
        {
            var result = await mediator.Send(new RemoveProjectMemberCommand(projectAndMember.ProjectId, new(projectAndMember.MemberId)), cancellationToken);
            if(result.IsFailed)
            {
                logger.LogCritical("Removing user (ID: {@userId}) from organization (ID: {@organizationId}) failed!", userId, organizationId);
                // TODO: throw?
            }

            var count = await dbContext.ProjectMembers.CountAsync(cancellationToken);
        }
    }

    public async Task ExpireOrganizationsInvitations(CancellationToken cancellationToken = default)
    {
        var now = dateTimeProvider.Now();
        var organizationsIds = await dbContext.OrganizationInvitations
            .Where(x => x.ExpirationDate.HasValue && x.ExpirationDate.Value < now)
            .Select(x => x.OrganizationId)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach(var organizationId in organizationsIds)
        {
            var organization = (await organizationRepository.GetById(organizationId, cancellationToken))!;
            organization.ExpireInvitations(now);
            await organizationRepository.Update(organization, cancellationToken);
        }
    }
}
