using Analytics.Infrastructure;
using Analytics.Infrastructure.Models;
using Domain.Common;
using System.Linq.Expressions;

namespace Analytics.Services;

public interface IProjectionHandler
{
    Task InitializeState(Guid projectId, bool rebuild = false);
    void ApplyEvent(DomainEvent domainEvent);
}

public abstract class ProjectionHandler<TProjection>(IRepository repository)
    : IProjectionHandler
    where TProjection : class, IProjection
{
    private List<TProjection> Projections { get; set; } = [];

    public async Task InitializeState(Guid projectId, bool rebuild = false)
    {
        if(rebuild)
        {
            await repository.ClearProjections<TProjection>(projectId);
            Projections.Clear();
        }
        else
        {
            Projections = (await repository.GetProjections<TProjection>(projectId)).ToList();
        }
    }

    public abstract void ApplyEvent(DomainEvent domainEvent);

    protected void Add(TProjection projection)
    {
        Projections.Add(projection);
        repository.AddProjection(projection);
    }

    protected TProjection? Find(Expression<Func<TProjection, bool>> predicate)
    {
        return Projections.FirstOrDefault(predicate.Compile());
    }

    protected void Remove(TProjection projection)
    {
        Projections.Remove(projection);
        repository.RemoveProjection(projection);
    }
}