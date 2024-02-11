using Application.Features.Projects;
using Microsoft.Extensions.Logging;

namespace Application.Common;

public interface IJobsService
{
    Task RemoveUserFromOrganizationProjects(Guid userId, Guid organizationId);
}

public class JobsService : IJobsService
{
    private readonly AppDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<JobsService> _logger;

    public JobsService(AppDbContext dbContext, IMediator mediator, ILogger<JobsService> logger)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task RemoveUserFromOrganizationProjects(Guid userId, Guid organizationId)
    {
        var projectsAndMembers = await _dbContext.Projects
            .Where(x => x.OrganizationId == organizationId && x.Members.Any(xx => xx.UserId == userId))
            .Select(v => new { ProjectId = v.Id, MemberId = v.Members.First(x => x.UserId == userId).Id })
            .ToListAsync();

        foreach (var projectAndMember in projectsAndMembers)
        {
            var result = await _mediator.Send(new RemoveProjectMemberCommand(projectAndMember.ProjectId, projectAndMember.MemberId));
            if(result.IsFailed)
            {
                _logger.LogCritical("Removing user (ID: {@userId}) from organization (ID: {@organizationId}) failed!", userId, organizationId);
                // TODO: throw?
            }

            var count = await _dbContext.ProjectMembers.CountAsync();
        }
    }
}
