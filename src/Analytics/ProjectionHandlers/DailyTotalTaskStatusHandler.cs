using Analytics.Infrastructure.Models;
using Analytics.Services;
using Domain.Common;
using Domain.Events;

namespace Analytics.ProjectionHandlers;

public class DailyTotalTaskStatusHandler(IRepository repository)
    : DailyTotalTaskPropertyHandler<DailyTotalTaskStatus, Guid>(repository, (x, property) => x.StatusId == property)
{
    public override void ApplyEvent(DomainEvent domainEvent)
    {
        if (domainEvent is TaskCreated taskCreated)
        {
            UpdateStatusCount(taskCreated.ProjectId, taskCreated.StatusId, taskCreated.OccurredAt.Date);
        }
        else if (domainEvent is TaskStatusUpdated taskStatusUpdated)
        {
            UpdateStatusCount(taskStatusUpdated.ProjectId, taskStatusUpdated.NewStatusId, taskStatusUpdated.OccurredAt.Date);
            UpdateStatusCount(taskStatusUpdated.ProjectId, taskStatusUpdated.OldStatusId, taskStatusUpdated.OccurredAt.Date, false);
        }
    }

    protected override DailyTotalTaskStatus CreateProjection(Guid projectId, DateTime date, Guid property, int count)
    {
        return new DailyTotalTaskStatus()
        {
            ProjectId = projectId,
            Date = date,
            StatusId = property,
            Count = count
        };
    }
}