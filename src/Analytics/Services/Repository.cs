using Analytics.Infrastructure;
using Analytics.Infrastructure.Models;

namespace Analytics.Services;

public interface IRepository
{
    void AddEvent(Event @event);
    void AddProjection<TProjection>(TProjection projection) 
        where TProjection : class, IProjection;
    Task Commit();
    Task<IReadOnlyList<Event>> GetEvents(Guid projectId);
    Task<IReadOnlyList<TProjection>> GetProjections<TProjection>(Guid projectId) 
        where TProjection : class, IProjection;
    Task<IReadOnlyList<Guid>> GetProjectsIds();
    void RemoveProjection<TProjection>(TProjection projection) 
        where TProjection : class, IProjection;
}

public class Repository(AnalyticsDbContext dbContext) : IRepository
{
    public async Task<IReadOnlyList<TProjection>> GetProjections<TProjection>(Guid projectId)
        where TProjection : class, IProjection
    {
        return await dbContext.Set<TProjection>()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Guid>> GetProjectsIds()
    {
        return await dbContext.Events
            .Select(x => x.ProjectId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Event>> GetEvents(Guid projectId)
    {
        return await dbContext.Events
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.OccurredAt)
            .ToListAsync();
    }

    public void AddProjection<TProjection>(TProjection projection)
        where TProjection : class, IProjection
    {
        dbContext.Set<TProjection>().Add(projection);
    }

    public void AddEvent(Event @event)
    {
        dbContext.Events.Add(@event);
    }

    public void RemoveProjection<TProjection>(TProjection projection)
        where TProjection : class, IProjection
    {
        dbContext.Set<TProjection>().Remove(projection);
    }

    public async Task Commit()
    {
        await dbContext.SaveChangesAsync();
    }
}
