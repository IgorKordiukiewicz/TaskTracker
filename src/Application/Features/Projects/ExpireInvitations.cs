using Application.Common;
using Domain.Projects;

namespace Application.Features.Projects;

public record ExpireProjectsInvitationsCommand() : IRequest;

internal class ExpireProjectsInvitationsHandler(AppDbContext dbContext, IRepository<Project> projectRepository, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<ExpireProjectsInvitationsCommand>
{
    public async Task Handle(ExpireProjectsInvitationsCommand request, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.Now();
        var projectsIds = await dbContext.ProjectInvitations
            .Where(x => x.ExpirationDate.HasValue && x.ExpirationDate.Value < now)
            .Select(x => x.ProjectId)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var projectId in projectsIds)
        {
            var project = (await projectRepository.GetById(projectId, cancellationToken))!;
            project.ExpireInvitations(now);
            await projectRepository.Update(project, cancellationToken);
        }
    }
}