using Analytics.Infrastructure;
using Analytics.Infrastructure.Models;
using Analytics.ProjectionHandlers;
using Domain.Common;
using System.Text.Json;

namespace Analytics.Services;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent);
}

public class DomainEventDispatcher(AnalyticsDbContext dbContext) 
    : IDomainEventDispatcher
{
    // TODO: add all projection handlers using reflection
    private readonly List<IProjectionHandler> _projectionHandlers = 
    [
        new DailyTotalTaskStatusHandler(dbContext),
    ];

    public async Task Dispatch(DomainEvent domainEvent)
    {
        foreach (var projectionHandler in _projectionHandlers)
        {
            await projectionHandler.ApplyEvent(domainEvent);
        }

        await SaveEvent(domainEvent);
    }

    private async Task SaveEvent(DomainEvent domainEvent)
    {
        var @event = new Event()
        {
            ProjectId = domainEvent.ProjectId,
            Details = JsonSerializer.Serialize(domainEvent),
            OccurredAt = domainEvent.OccurredAt
        };

        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
    }
}