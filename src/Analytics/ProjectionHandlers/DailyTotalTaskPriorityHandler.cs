using Analytics.Infrastructure.Models;
using Analytics.Services;
using Domain.Common;
using Domain.Enums;
using Domain.Events;

namespace Analytics.ProjectionHandlers;

public class DailyTotalTaskPriorityHandler(IRepository repository)
    : DailyTotalTaskPropertyHandler<DailyTotalTaskPriority, TaskPriority>(repository, (x, property) => x.Priority == property)
{
    public override void ApplyEvent(DomainEvent domainEvent)
    {
        if (domainEvent is TaskCreated taskCreated)
        {
            UpdateStatusCount(taskCreated.ProjectId, taskCreated.Priority, taskCreated.OccurredAt.Date);
        }
        else if (domainEvent is TaskPriorityUpdated taskPriorityUpdated)
        {
            UpdateStatusCount(taskPriorityUpdated.ProjectId, taskPriorityUpdated.NewPriority, taskPriorityUpdated.OccurredAt.Date);
            UpdateStatusCount(taskPriorityUpdated.ProjectId, taskPriorityUpdated.OldPriority, taskPriorityUpdated.OccurredAt.Date, false);
        }
    }

    protected override DailyTotalTaskPriority CreateProjection(Guid projectId, DateTime date, TaskPriority property, int count)
    {
        return new DailyTotalTaskPriority()
        {
            ProjectId = projectId,
            Date = date,
            Priority = property,
            Count = count
        };
    }
}

