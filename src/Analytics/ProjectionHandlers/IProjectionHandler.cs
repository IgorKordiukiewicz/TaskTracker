using Domain.Common;

namespace Analytics.ProjectionHandlers;

public interface IProjectionHandler
{
    Task ApplyEvent(DomainEvent domainEvent);
}
