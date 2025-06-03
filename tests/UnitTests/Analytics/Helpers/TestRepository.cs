using Analytics.Infrastructure.Models;
using Analytics.Services;

namespace UnitTests.Analytics.Helpers;

public class TestRepository : IRepository
{
    public List<Event> Events { get; set; } = [];
    public List<IProjection> Projections { get; set; } = [];

    public void AddEvent(Event @event)
    {
        Events.Add(@event);
    }

    public Task Commit()
    {
        // Nothing to do
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Event>> GetEvents(Guid projectId)
    {
        return Task.FromResult<IReadOnlyList<Event>>(
            Events.Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.OccurredAt)
            .ToList());
    }

    public Task<IReadOnlyList<Guid>> GetProjectsIds()
    {
        return Task.FromResult<IReadOnlyList<Guid>>(
            Events.Select(x => x.ProjectId)
            .Distinct()
            .ToList());
    }

    void IRepository.AddProjection<TProjection>(TProjection projection)
    {
        Projections.Add(projection);
    }

    Task IRepository.ClearProjections<TProjection>(Guid projectId)
    {
        Projections.Clear();
        return Task.CompletedTask;
    }

    Task<IReadOnlyList<TProjection>> IRepository.GetProjections<TProjection>(Guid projectId)
    {
        return Task.FromResult<IReadOnlyList<TProjection>>(
            Projections.OfType<TProjection>()
            .Where(x => x.ProjectId == projectId)
            .ToList());
    }

    void IRepository.RemoveProjection<TProjection>(TProjection projection)
    {
        Projections.Remove(projection);
    }
}