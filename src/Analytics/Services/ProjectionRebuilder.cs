using Analytics.Infrastructure;
using Domain.Common;

namespace Analytics.Services;

public interface IProjectionRebuilder
{
    Task RebuildProjections();
}

public class ProjectionRebuilder(IRepository repository, IEnumerable<IProjectionHandler> projectionHandlers) 
    : IProjectionRebuilder
{
    public async Task RebuildProjections()
    {
        var projectIds = await repository.GetProjectsIds();

        foreach(var projectId in projectIds)
        {
            await RebuildProjectProjections(projectId);
        }
    }

    private async Task RebuildProjectProjections(Guid projectId)
    {
        var domainEvents = (await repository.GetEvents(projectId))
            .Select(x => x.ToDomainEvent())
            .ToList();

        foreach (var projectionHandler in projectionHandlers)
        {
            await projectionHandler.InitializeState(projectId, true);
            foreach (var domainEvent in domainEvents)
            {
                projectionHandler.ApplyEvent(domainEvent!);
            }
        }

        // Changes are saved per project
        await repository.Commit();
    }
}
