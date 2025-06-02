using Domain.Common;

namespace Analytics.Services;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent);
}

public class DomainEventDispatcher(IRepository repository, IEnumerable<IProjectionHandler> projectionHandlers) 
    : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent)
    {
        foreach (var projectionHandler in projectionHandlers)
        {
            await projectionHandler.InitializeState(domainEvent.ProjectId);
            projectionHandler.ApplyEvent(domainEvent);
        }

        // Event is saved with updated projections in a transaction
        repository.AddEvent(domainEvent.ToEvent());
        await repository.Commit();
    }
}